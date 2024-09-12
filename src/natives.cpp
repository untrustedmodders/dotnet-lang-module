#include "core.h"
#include "memory.h"
#include <dyncall/dyncall.h>
#include <plugify/string.h>
#include <module_export.h>

using namespace netlm;

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
		{type_id<DCCallVM>, "DCCallVM"},
		{type_id<DCaggr>, "DCAggr"}
	};
	auto it = typeNameMap.find(type);
	if (it != typeNameMap.end()) {
		return std::get<std::string_view>(*it);
	}
	return "unknown";
}

template<typename T>
std::vector<T>* CreateVector(T* arr, int len) requires(!std::is_same_v<T, char*>) {
	auto vector = len == 0 ? new std::vector<T>() : new std::vector<T>(arr, arr + len);
	assert(vector);
	++g_numberOfAllocs[type_id<std::vector<T>>];
	return vector;
}

template<typename T>
std::vector<plg::string>* CreateVector(T* arr, int len) requires(std::is_same_v<T, char*>) {
	auto vector = len == 0 ? new std::vector<plg::string>() : new std::vector<plg::string>(arr, arr + len);
	assert(vector);
	++g_numberOfAllocs[type_id<std::vector<plg::string>>];
	return vector;
}

template<typename T>
std::vector<T>* AllocateVector() requires(!std::is_same_v<T, char*>) {
	auto vector = static_cast<std::vector<T>*>(std::malloc(sizeof(std::vector<T>)));
	assert(vector);
	++g_numberOfMalloc[type_id<std::vector<T>>];
	return vector;
}

template<typename T>
std::vector<plg::string>* AllocateVector() requires(std::is_same_v<T, char*>) {
	auto vector = static_cast<std::vector<plg::string>*>(std::malloc(sizeof(std::vector<plg::string>)));
	assert(vector);
	++g_numberOfMalloc[type_id<std::vector<plg::string>>];
	return vector;
}

template<typename T>
void DeleteVector(std::vector<T>* vector) {
	delete vector;
	--g_numberOfAllocs[type_id<std::vector<T>>];
	assert(g_numberOfAllocs[type_id<std::vector<T>>] != -1);
}

template<typename T>
void FreeVector(std::vector<T>* vector) {
	vector->~vector();
	std::free(vector);
	--g_numberOfMalloc[type_id<std::vector<T>>];
	assert(g_numberOfMalloc[type_id<std::vector<T>>] != -1);
}

template<typename T>
int GetVectorSize(std::vector<T>* vector) {
	return static_cast<int>(vector->size());
}

template<typename T>
void AssignVector(std::vector<T>* vector, T* arr, int len) requires(!std::is_same_v<T, char*>) {
	if (arr == nullptr || len == 0)
		vector->clear();
	else
		vector->assign(arr, arr + len);
}

template<typename T>
void AssignVector(std::vector<plg::string>* vector, T* arr, int len) requires(std::is_same_v<T, char*>) {
	if (arr == nullptr || len == 0)
		vector->clear();
	else
		vector->assign(arr, arr + len);
}

template<typename T>
void GetVectorData(std::vector<T>* vector, T* arr) requires(!std::is_same_v<T, char*>) {
	for (size_t i = 0; i < vector->size(); ++i) {
		arr[i] = (*vector)[i];
	}
}

template<typename T>
void GetVectorData(std::vector<plg::string>* vector, T* arr) requires(std::is_same_v<T, char*>) {
	for (size_t i = 0; i < vector->size(); ++i) {
		Memory::FreeCoTaskMem(arr[i]);
		arr[i] = Memory::StringToHGlobalAnsi((*vector)[i]);
	}
}

template<typename T>
void ConstructVector(std::vector<T>* vector, T* arr, int len) requires(!std::is_same_v<T, char*>) {
	std::construct_at(vector, len == 0 ? std::vector<T>() : std::vector<T>(arr, arr + len));
}

template<typename T>
void ConstructVector(std::vector<plg::string>* vector, T* arr, int len) requires(std::is_same_v<T, char*>) {
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

	NETLM_EXPORT void ConstructVectorDataBool(std::vector<bool>* vector, bool* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataChar8(std::vector<char>* vector, char* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataChar16(std::vector<char16_t>* vector, char16_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataInt8(std::vector<int8_t>* vector, int8_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataInt16(std::vector<int16_t>* vector, int16_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataInt32(std::vector<int32_t>* vector, int32_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataInt64(std::vector<int64_t>* vector, int64_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataUInt8(std::vector<uint8_t>* vector, uint8_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataUInt16(std::vector<uint16_t>* vector, uint16_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataUInt32(std::vector<uint32_t>* vector, uint32_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataUInt64(std::vector<uint64_t>* vector, uint64_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataIntPtr(std::vector<uintptr_t>* vector, uintptr_t* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataFloat(std::vector<float>* vector, float* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataDouble(std::vector<double>* vector, double* arr, int len) { ConstructVector(vector, arr, len); }
	NETLM_EXPORT void ConstructVectorDataString(std::vector<plg::string>* vector, char* arr[], int len)  { ConstructVector(vector, arr, len); }

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

	// Dyncall Functions

	NETLM_EXPORT DCCallVM* NewVM(size_t size) {
		DCCallVM* vm = dcNewCallVM(size);
		++g_numberOfAllocs[type_id<DCCallVM>];
		return vm;
	}
	NETLM_EXPORT void Free(DCCallVM* vm) {
		dcFree(vm);
		--g_numberOfAllocs[type_id<DCCallVM>];
		assert(g_numberOfAllocs[type_id<DCCallVM>] != -1);
	}
	NETLM_EXPORT void Reset(DCCallVM* vm) { dcReset(vm); }
	NETLM_EXPORT void Mode(DCCallVM* vm, int mode) { dcMode(vm, mode); }

	NETLM_EXPORT void ArgBool(DCCallVM* vm, bool value) { dcArgBool(vm, value); }
	NETLM_EXPORT void ArgChar8(DCCallVM* vm, char value) { dcArgChar(vm, value); }
	NETLM_EXPORT void ArgChar16(DCCallVM* vm, char16_t value) { dcArgShort(vm, static_cast<short>(value)); }
	NETLM_EXPORT void ArgInt8(DCCallVM* vm, int8_t value) { dcArgChar(vm, value); }
	NETLM_EXPORT void ArgUInt8(DCCallVM* vm, uint8_t value) { dcArgChar(vm, static_cast<int8_t>(value)); }
	NETLM_EXPORT void ArgInt16(DCCallVM* vm, int16_t value) { dcArgShort(vm, value); }
	NETLM_EXPORT void ArgUInt16(DCCallVM* vm, uint16_t value) { dcArgShort(vm, static_cast<int16_t>(value)); }
	NETLM_EXPORT void ArgInt32(DCCallVM* vm, int32_t value) { dcArgInt(vm, value); }
	NETLM_EXPORT void ArgUInt32(DCCallVM* vm, uint32_t value) { dcArgInt(vm, static_cast<int32_t>(value)); }
	NETLM_EXPORT void ArgInt64(DCCallVM* vm, int64_t value) { dcArgLongLong(vm, value); }
	NETLM_EXPORT void ArgUInt64(DCCallVM* vm, uint64_t value) { dcArgLongLong(vm, static_cast<int64_t>(value)); }
	NETLM_EXPORT void ArgFloat(DCCallVM* vm, float value) { dcArgFloat(vm, value); }
	NETLM_EXPORT void ArgDouble(DCCallVM* vm, double value) { dcArgDouble(vm, value); }
	NETLM_EXPORT void ArgPointer(DCCallVM* vm, void* value) { dcArgPointer(vm, value); }
	NETLM_EXPORT void ArgAggr(DCCallVM* vm, DCaggr* ag, void* value) { dcArgAggr(vm, ag, value); }

	NETLM_EXPORT void CallVoid(DCCallVM* vm, void* funcptr) { dcCallVoid(vm, funcptr); }
	NETLM_EXPORT bool CallBool(DCCallVM* vm, void* funcptr) { return dcCallBool(vm, funcptr); }
	NETLM_EXPORT char CallChar8(DCCallVM* vm, void* funcptr) { return dcCallChar(vm, funcptr); }
	NETLM_EXPORT char16_t CallChar16(DCCallVM* vm, void* funcptr) { return static_cast<char16_t>(dcCallShort(vm, funcptr)); }
	NETLM_EXPORT int8_t CallInt8(DCCallVM* vm, void* funcptr) { return dcCallChar(vm, funcptr); }
	NETLM_EXPORT uint8_t CallUInt8(DCCallVM* vm, void* funcptr) { return static_cast<uint8_t>(dcCallChar(vm, funcptr)); }
	NETLM_EXPORT int16_t CallInt16(DCCallVM* vm, void* funcptr) { return dcCallShort(vm, funcptr); }
	NETLM_EXPORT uint16_t CallUInt16(DCCallVM* vm, void* funcptr) { return static_cast<uint16_t>(dcCallShort(vm, funcptr)); }
	NETLM_EXPORT int32_t CallInt32(DCCallVM* vm, void* funcptr) { return dcCallInt(vm, funcptr); }
	NETLM_EXPORT uint32_t CallUInt32(DCCallVM* vm, void* funcptr) { return static_cast<uint32_t>(dcCallInt(vm, funcptr)); }
	NETLM_EXPORT int64_t CallInt64(DCCallVM* vm, void* funcptr) { return dcCallLongLong(vm, funcptr); }
	NETLM_EXPORT uint64_t CallUInt64(DCCallVM* vm, void* funcptr) { return static_cast<uint64_t>(dcCallLongLong(vm, funcptr)); }
	NETLM_EXPORT float CallFloat(DCCallVM* vm, void* funcptr) { return dcCallFloat(vm, funcptr); }
	NETLM_EXPORT double CallDouble(DCCallVM* vm, void* funcptr) { return dcCallDouble(vm, funcptr); }
	NETLM_EXPORT void* CallPointer(DCCallVM* vm, void* funcptr) { return dcCallPointer(vm, funcptr); }
	NETLM_EXPORT void CallAggr(DCCallVM* vm, void* funcptr, DCaggr* ag, void* returnValue) { dcCallAggr(vm, funcptr, ag, returnValue); }

	NETLM_EXPORT void BeginCallAggr(DCCallVM* vm, DCaggr* ag) { dcBeginCallAggr(vm, ag); }

	NETLM_EXPORT int GetError(DCCallVM* vm) { return dcGetError(vm); }

	NETLM_EXPORT DCaggr* NewAggr(size_t fieldCount, size_t size) {
		DCaggr* ag = dcNewAggr(fieldCount, size);
		++g_numberOfAllocs[type_id<DCaggr>];
		return ag;
	}
	NETLM_EXPORT void FreeAggr(DCaggr* ag) {
		dcFreeAggr(ag);
		--g_numberOfAllocs[type_id<DCaggr>];
		assert(g_numberOfAllocs[type_id<DCaggr>] != -1);
	}
	NETLM_EXPORT void AggrField(DCaggr* ag, char type, int offset, size_t arrayLength) { dcAggrField(ag, type, offset, arrayLength); }
	NETLM_EXPORT void CloseAggr(DCaggr* ag) { dcCloseAggr(ag); }

	NETLM_EXPORT int GetModeFromCCSigChar(char sigChar) { return dcGetModeFromCCSigChar(sigChar); }
}
