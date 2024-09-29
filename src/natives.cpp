#include "core.h"
#include "memory.h"
#include "module.h"
#include "managed_type.h"
#include <module_export.h>
#include <plugify/jit/call.h>
#include <plugify/jit/helpers.h>
#include <plugify/string.h>

using namespace netlm;
using namespace plugify;

std::map<type_index, int32_t> g_numberOfMalloc = { };
std::map<type_index, int32_t> g_numberOfAllocs = { };

std::string_view GetTypeName(type_index type) {
	static std::map<type_index, std::string_view> typeNameMap = {
		{type_id<plg::string>, "String"},
		{type_id<std::vector<bool>>, "VectorBool"},
		{type_id<std::vector<char>>, "VectorChar8"},
		{type_id<std::vector<char16_t>>, "VectorChar16"},
		{type_id<std::vector<int8_t>>, "VectorInt8"},
		{type_id<std::vector<int16_t>>, "VectorInt16"},
		{type_id<std::vector<int32_t>>, "VectorInt32"},
		{type_id<std::vector<int64_t>>, "VectorInt64"},
		{type_id<std::vector<uint8_t>>, "VectorUInt8"},
		{type_id<std::vector<uint16_t>>, "VectorUInt16"},
		{type_id<std::vector<uint32_t>>, "VectorUInt32"},
		{type_id<std::vector<uint64_t>>, "VectorUInt64"},
		{type_id<std::vector<uintptr_t>>, "VectorUIntPtr"},
		{type_id<std::vector<float>>, "VectorFloat"},
		{type_id<std::vector<double>>, "VectorDouble"},
		{type_id<std::vector<plg::string>>, "VectorString"},
		{type_id<JitCall>, "JitCall"},
	};
	auto it = typeNameMap.find(type);
	if (it != typeNameMap.end()) {
		return std::get<std::string_view>(*it);
	}
	return "unknown";
}

template<typename T>
static std::vector<T>* CreateVector(T* arr, int len) requires(!std::is_same_v<T, char*>) {
	auto vector = len == 0 ? new std::vector<T>() : new std::vector<T>(arr, arr + len);
	assert(vector);
	++g_numberOfAllocs[type_id<std::vector<T>>];
	return vector;
}

template<typename T>
static std::vector<plg::string>* CreateVector(T* arr, int len) requires(std::is_same_v<T, char*>) {
	auto vector = len == 0 ? new std::vector<plg::string>() : new std::vector<plg::string>(arr, arr + len);
	assert(vector);
	++g_numberOfAllocs[type_id<std::vector<plg::string>>];
	return vector;
}

template<typename T>
static std::vector<T>* AllocateVector() requires(!std::is_same_v<T, char*>) {
	auto vector = static_cast<std::vector<T>*>(std::malloc(sizeof(std::vector<T>)));
	assert(vector);
	++g_numberOfMalloc[type_id<std::vector<T>>];
	return vector;
}

template<typename T>
static std::vector<plg::string>* AllocateVector() requires(std::is_same_v<T, char*>) {
	auto vector = static_cast<std::vector<plg::string>*>(std::malloc(sizeof(std::vector<plg::string>)));
	assert(vector);
	++g_numberOfMalloc[type_id<std::vector<plg::string>>];
	return vector;
}

template<typename T>
static void DeleteVector(std::vector<T>* vector) {
	delete vector;
	--g_numberOfAllocs[type_id<std::vector<T>>];
	assert(g_numberOfAllocs[type_id<std::vector<T>>] != -1);
}

template<typename T>
static void FreeVector(std::vector<T>* vector) {
	vector->~vector();
	std::free(vector);
	--g_numberOfMalloc[type_id<std::vector<T>>];
	assert(g_numberOfMalloc[type_id<std::vector<T>>] != -1);
}

template<typename T>
static int GetVectorSize(std::vector<T>* vector) {
	return static_cast<int>(vector->size());
}

template<typename T>
static void AssignVector(std::vector<T>* vector, T* arr, int len) requires(!std::is_same_v<T, char*>) {
	if (arr == nullptr || len == 0)
		vector->clear();
	else
		vector->assign(arr, arr + len);
}

template<typename T>
static void AssignVector(std::vector<plg::string>* vector, T* arr, int len) requires(std::is_same_v<T, char*>) {
	if (arr == nullptr || len == 0)
		vector->clear();
	else
		vector->assign(arr, arr + len);
}

template<typename T>
static void GetVectorData(std::vector<T>* vector, T* arr) requires(!std::is_same_v<T, char*>) {
	for (size_t i = 0; i < vector->size(); ++i) {
		arr[i] = (*vector)[i];
	}
}

template<typename T>
static void GetVectorData(std::vector<plg::string>* vector, T* arr) requires(std::is_same_v<T, char*>) {
	for (size_t i = 0; i < vector->size(); ++i) {
		Memory::FreeCoTaskMem(arr[i]);
		arr[i] = Memory::StringToHGlobalAnsi((*vector)[i]);
	}
}

template<typename T>
static void ConstructVector(std::vector<T>* vector, T* arr, int len) requires(!std::is_same_v<T, char*>) {
	std::construct_at(vector, len == 0 ? std::vector<T>() : std::vector<T>(arr, arr + len));
}

template<typename T>
static void ConstructVector(std::vector<plg::string>* vector, T* arr, int len) requires(std::is_same_v<T, char*>) {
	std::construct_at(vector, len == 0 ? std::vector<plg::string>() : std::vector<plg::string>(arr, arr + len));
}

extern "C" {
	// String Functions

	NETLM_EXPORT plg::string* AllocateString() {
		auto str = static_cast<plg::string*>(std::malloc(sizeof(plg::string)));
		++g_numberOfMalloc[type_id<plg::string>];
		return str;
	}
	NETLM_EXPORT void* CreateString(const char* source) {
		auto str = source == nullptr ? new plg::string() : new plg::string(source);
		++g_numberOfAllocs[type_id<plg::string>];
		return str;
	}
	NETLM_EXPORT const char* GetStringData(plg::string* string) {
		return Memory::StringToHGlobalAnsi(*string);
	}
	NETLM_EXPORT int GetStringLength(plg::string* string) {
		return static_cast<int>(string->length());
	}
	NETLM_EXPORT void ConstructString(plg::string* string, const char* source) {
		if (source == nullptr)
			std::construct_at(string, plg::string());
		else
			std::construct_at(string, source);
	}
	NETLM_EXPORT void AssignString(plg::string* string, const char* source) {
		if (source == nullptr)
			string->clear();
		else
			string->assign(source);
	}
	NETLM_EXPORT void FreeString(plg::string* string) {
		string->~basic_string();
		std::free(string);
		--g_numberOfMalloc[type_id<plg::string>];
		assert(g_numberOfMalloc[type_id<plg::string>] != -1);
	}
	NETLM_EXPORT void DeleteString(plg::string* string) {
		delete string;
		--g_numberOfAllocs[type_id<plg::string>];
		assert(g_numberOfAllocs[type_id<plg::string>] != -1);
	}

	// CreateVector Functions
	NETLM_EXPORT std::vector<bool>* CreateVectorBool(bool* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<char>* CreateVectorChar8(char* arr, int len)  { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<char16_t>* CreateVectorChar16(char16_t* arr, int len)  { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<int8_t>* CreateVectorInt8(int8_t* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<int16_t>* CreateVectorInt16(int16_t* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<int32_t>* CreateVectorInt32(int32_t* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<int64_t>* CreateVectorInt64(int64_t* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<uint8_t>* CreateVectorUInt8(uint8_t* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<uint16_t>* CreateVectorUInt16(uint16_t* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<uint32_t>* CreateVectorUInt32(uint32_t* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<uint64_t>* CreateVectorUInt64(uint64_t* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<uintptr_t>* CreateVectorIntPtr(uintptr_t* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<float>* CreateVectorFloat(float* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<double>* CreateVectorDouble(double* arr, int len) { return CreateVector(arr, len); }
	NETLM_EXPORT std::vector<plg::string>* CreateVectorString(char* arr[], int len) { return CreateVector(arr, len); }

	// AllocateVector Functions
	NETLM_EXPORT std::vector<bool>* AllocateVectorBool() { return AllocateVector<bool>(); }
	NETLM_EXPORT std::vector<char>* AllocateVectorChar8() { return AllocateVector<char>(); }
	NETLM_EXPORT std::vector<char16_t>* AllocateVectorChar16() { return AllocateVector<char16_t>(); }
	NETLM_EXPORT std::vector<int8_t>* AllocateVectorInt8() { return AllocateVector<int8_t>(); }
	NETLM_EXPORT std::vector<int16_t>* AllocateVectorInt16() { return AllocateVector<int16_t>(); }
	NETLM_EXPORT std::vector<int32_t>* AllocateVectorInt32() { return AllocateVector<int32_t>(); }
	NETLM_EXPORT std::vector<int64_t>* AllocateVectorInt64() { return AllocateVector<int64_t>(); }
	NETLM_EXPORT std::vector<uint8_t>* AllocateVectorUInt8() { return AllocateVector<uint8_t>(); }
	NETLM_EXPORT std::vector<uint16_t>* AllocateVectorUInt16() { return AllocateVector<uint16_t>(); }
	NETLM_EXPORT std::vector<uint32_t>* AllocateVectorUInt32() { return AllocateVector<uint32_t>(); }
	NETLM_EXPORT std::vector<uint64_t>* AllocateVectorUInt64()  { return AllocateVector<uint64_t>(); }
	NETLM_EXPORT std::vector<uintptr_t>* AllocateVectorIntPtr() { return AllocateVector<uintptr_t>(); }
	NETLM_EXPORT std::vector<float>* AllocateVectorFloat() { return AllocateVector<float>(); }
	NETLM_EXPORT std::vector<double>* AllocateVectorDouble() { return AllocateVector<double>(); }
	NETLM_EXPORT std::vector<plg::string>* AllocateVectorString() { return AllocateVector<char*>(); }


	// GetVectorSize Functions
	NETLM_EXPORT int GetVectorSizeBool(std::vector<bool>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeChar8(std::vector<char>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeChar16(std::vector<char16_t>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeInt8(std::vector<int8_t>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeInt16(std::vector<int16_t>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeInt32(std::vector<int32_t>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeInt64(std::vector<int64_t>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeUInt8(std::vector<uint8_t>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeUInt16(std::vector<uint16_t>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeUInt32(std::vector<uint32_t>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeUInt64(std::vector<uint64_t>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeIntPtr(std::vector<uintptr_t>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeFloat(std::vector<float>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeDouble(std::vector<double>* vector) { return GetVectorSize(vector); }
	NETLM_EXPORT int GetVectorSizeString(std::vector<plg::string>* vector) { return GetVectorSize(vector); }

	// GetVectorData Functions

	NETLM_EXPORT void GetVectorDataBool(std::vector<bool>* vector, bool* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataChar8(std::vector<char>* vector, char* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataChar16(std::vector<char16_t>* vector, char16_t* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataInt8(std::vector<int8_t>* vector, int8_t* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataInt16(std::vector<int16_t>* vector, int16_t* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataInt32(std::vector<int32_t>* vector, int32_t* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataInt64(std::vector<int64_t>* vector, int64_t* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataUInt8(std::vector<uint8_t>* vector, uint8_t* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataUInt16(std::vector<uint16_t>* vector, uint16_t* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataUInt32(std::vector<uint32_t>* vector, uint32_t* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataUInt64(std::vector<uint64_t>* vector, uint64_t* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataIntPtr(std::vector<uintptr_t>* vector, uintptr_t* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataFloat(std::vector<float>* vector, float* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataDouble(std::vector<double>* vector, double* arr) { GetVectorData(vector, arr); }
	NETLM_EXPORT void GetVectorDataString(std::vector<plg::string>* vector, char* arr[]) { GetVectorData(vector, arr); }

	// Construct Functions

	NETLM_EXPORT void ConstructVectorBool(std::vector<bool>* vector, bool* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorChar8(std::vector<char>* vector, char* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorChar16(std::vector<char16_t>* vector, char16_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorInt8(std::vector<int8_t>* vector, int8_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorInt16(std::vector<int16_t>* vector, int16_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorInt32(std::vector<int32_t>* vector, int32_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorInt64(std::vector<int64_t>* vector, int64_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorUInt8(std::vector<uint8_t>* vector, uint8_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorUInt16(std::vector<uint16_t>* vector, uint16_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorUInt32(std::vector<uint32_t>* vector, uint32_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorUInt64(std::vector<uint64_t>* vector, uint64_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorIntPtr(std::vector<uintptr_t>* vector, uintptr_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorFloat(std::vector<float>* vector, float* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDouble(std::vector<double>* vector, double* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorString(std::vector<plg::string>* vector, char* arr[], int len)  { ConstructVector(vector, arr, len); }

	// AssignVector Functions

	NETLM_EXPORT void AssignVectorBool(std::vector<bool>* vector, bool* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorChar8(std::vector<char>* vector, char* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorChar16(std::vector<char16_t>* vector, char16_t* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorInt8(std::vector<int8_t>* vector, int8_t* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorInt16(std::vector<int16_t>* vector, int16_t* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorInt32(std::vector<int32_t>* vector, int32_t* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorInt64(std::vector<int64_t>* vector, int64_t* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorUInt8(std::vector<uint8_t>* vector, uint8_t* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorUInt16(std::vector<uint16_t>* vector, uint16_t* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorUInt32(std::vector<uint32_t>* vector, uint32_t* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorUInt64(std::vector<uint64_t>* vector, uint64_t* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorIntPtr(std::vector<uintptr_t>* vector, uintptr_t* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorFloat(std::vector<float>* vector, float* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorDouble(std::vector<double>* vector, double* arr, int len) { AssignVector(vector, arr, len); }
	NETLM_EXPORT void AssignVectorString(std::vector<plg::string>* vector, char* arr[], int len) { AssignVector(vector, arr, len); }

	// DeleteVector Functions

	NETLM_EXPORT void DeleteVectorBool(std::vector<bool>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorChar8(std::vector<char>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorChar16(std::vector<char16_t>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorInt8(std::vector<int8_t>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorInt16(std::vector<int16_t>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorInt32(std::vector<int32_t>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorInt64(std::vector<int64_t>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorUInt8(std::vector<uint8_t>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorUInt16(std::vector<uint16_t>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorUInt32(std::vector<uint32_t>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorUInt64(std::vector<uint64_t>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorIntPtr(std::vector<uintptr_t>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorFloat(std::vector<float>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorDouble(std::vector<double>* vector) { DeleteVector(vector); }
	NETLM_EXPORT void DeleteVectorString(std::vector<plg::string>* vector) { DeleteVector(vector); }

	// FreeVector functions

	NETLM_EXPORT void FreeVectorBool(std::vector<bool>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorChar8(std::vector<char>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorChar16(std::vector<char16_t>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorInt8(std::vector<int8_t>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorInt16(std::vector<int16_t>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorInt32(std::vector<int32_t>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorInt64(std::vector<int64_t>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorUInt8(std::vector<uint8_t>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorUInt16(std::vector<uint16_t>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorUInt32(std::vector<uint32_t>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorUInt64(std::vector<uint64_t>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorIntPtr(std::vector<uintptr_t>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorFloat(std::vector<float>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorDouble(std::vector<double>* vector) { FreeVector(vector); }
	NETLM_EXPORT void FreeVectorString(std::vector<plg::string>* vector) { FreeVector(vector); }
}

extern "C" {
	// Dyncall Functions

	NETLM_EXPORT JitCall* NewCall(void* target, ManagedType* params, int count, ManagedType ret) {
		if (target == nullptr)
			return nullptr;

		bool isHiddenParam = ValueUtils::IsHiddenParam(ret.type);
		asmjit::FuncSignature sig(asmjit::CallConvId::kHost, asmjit::FuncSignature::kNoVarArgs, JitUtils::GetRetTypeId(isHiddenParam ? ValueType::Pointer : ret.type));
		if (isHiddenParam) {
			sig.addArg(JitUtils::GetValueTypeId(ret.type));
		}

		for (int i = 0; i < count; ++i) {
			const auto& [type, ref] = params[i];
			sig.addArg(JitUtils::GetValueTypeId(ref ? ValueType::Pointer : type));
		}

		JitCall* call = new JitCall(g_netlm.GetRuntime());
		call->GetJitFunc(sig, target);
		++g_numberOfAllocs[type_id<JitCall>];
		return call;
	}

	NETLM_EXPORT void DeleteCall(JitCall* call) {
		delete call;
		--g_numberOfAllocs[type_id<JitCall>];
		assert(g_numberOfAllocs[type_id<JitCall>] != -1);
	}

	NETLM_EXPORT void* GetCallFunction(JitCall* call) {
		return call->GetFunction();
	}

	NETLM_EXPORT char* GetCallError(JitCall* call) {
		return Memory::StringToHGlobalAnsi(call->GetError().data());
	}
}
