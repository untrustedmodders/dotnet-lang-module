#pragma once

#include <plugify/cpp_plugin.h>
#include <vector>
#include <string>
#include <cstdint>

// Generated from CSharpTest.pplugin by https://github.com/untrustedmodders/cpp-lang-module/blob/main/generator/generator.py 

namespace CSharpTest {
	using MyFunc = void (*)(int32_t a, const std::string& b, const std::vector<int32_t>& c);
	inline void MyExportFunction(int32_t a, MyFunc func) {
		using MyExportFunctionFn = void (*)(int32_t, MyFunc);
		static MyExportFunctionFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.MyExportFunction", reinterpret_cast<void**>(&__func));
		__func(a, func);
	}
	inline void NoParamReturnVoid() {
		using NoParamReturnVoidFn = void (*)();
		static NoParamReturnVoidFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnVoid", reinterpret_cast<void**>(&__func));
		__func();
	}
	inline bool NoParamReturnBool() {
		using NoParamReturnBoolFn = bool (*)();
		static NoParamReturnBoolFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnBool", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline char NoParamReturnChar8() {
		using NoParamReturnChar8Fn = char (*)();
		static NoParamReturnChar8Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnChar8", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline char16_t NoParamReturnChar16() {
		using NoParamReturnChar16Fn = char16_t (*)();
		static NoParamReturnChar16Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnChar16", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline int8_t NoParamReturnInt8() {
		using NoParamReturnInt8Fn = int8_t (*)();
		static NoParamReturnInt8Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnInt8", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline int16_t NoParamReturnInt16() {
		using NoParamReturnInt16Fn = int16_t (*)();
		static NoParamReturnInt16Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnInt16", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline int32_t NoParamReturnInt32() {
		using NoParamReturnInt32Fn = int32_t (*)();
		static NoParamReturnInt32Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnInt32", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline int64_t NoParamReturnInt64() {
		using NoParamReturnInt64Fn = int64_t (*)();
		static NoParamReturnInt64Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnInt64", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline uint8_t NoParamReturnUInt8() {
		using NoParamReturnUInt8Fn = uint8_t (*)();
		static NoParamReturnUInt8Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnUInt8", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline uint16_t NoParamReturnUInt16() {
		using NoParamReturnUInt16Fn = uint16_t (*)();
		static NoParamReturnUInt16Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnUInt16", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline uint32_t NoParamReturnUInt32() {
		using NoParamReturnUInt32Fn = uint32_t (*)();
		static NoParamReturnUInt32Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnUInt32", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline uint64_t NoParamReturnUInt64() {
		using NoParamReturnUInt64Fn = uint64_t (*)();
		static NoParamReturnUInt64Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnUInt64", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline void* NoParamReturnPtr64() {
		using NoParamReturnPtr64Fn = void* (*)();
		static NoParamReturnPtr64Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnPtr64", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline float NoParamReturnFloat() {
		using NoParamReturnFloatFn = float (*)();
		static NoParamReturnFloatFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnFloat", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline double NoParamReturnDouble() {
		using NoParamReturnDoubleFn = double (*)();
		static NoParamReturnDoubleFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnDouble", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline void* NoParamReturnFunction() {
		using NoParamReturnFunctionFn = void* (*)();
		static NoParamReturnFunctionFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnFunction", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::string NoParamReturnString() {
		using NoParamReturnStringFn = std::string (*)();
		static NoParamReturnStringFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnString", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<bool> NoParamReturnArrayBool() {
		using NoParamReturnArrayBoolFn = std::vector<bool> (*)();
		static NoParamReturnArrayBoolFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayBool", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<char> NoParamReturnArrayChar8() {
		using NoParamReturnArrayChar8Fn = std::vector<char> (*)();
		static NoParamReturnArrayChar8Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayChar8", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<char16_t> NoParamReturnArrayChar16() {
		using NoParamReturnArrayChar16Fn = std::vector<char16_t> (*)();
		static NoParamReturnArrayChar16Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayChar16", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<int8_t> NoParamReturnArrayInt8() {
		using NoParamReturnArrayInt8Fn = std::vector<int8_t> (*)();
		static NoParamReturnArrayInt8Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayInt8", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<int16_t> NoParamReturnArrayInt16() {
		using NoParamReturnArrayInt16Fn = std::vector<int16_t> (*)();
		static NoParamReturnArrayInt16Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayInt16", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<int32_t> NoParamReturnArrayInt32() {
		using NoParamReturnArrayInt32Fn = std::vector<int32_t> (*)();
		static NoParamReturnArrayInt32Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayInt32", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<int64_t> NoParamReturnArrayInt64() {
		using NoParamReturnArrayInt64Fn = std::vector<int64_t> (*)();
		static NoParamReturnArrayInt64Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayInt64", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<uint8_t> NoParamReturnArrayUInt8() {
		using NoParamReturnArrayUInt8Fn = std::vector<uint8_t> (*)();
		static NoParamReturnArrayUInt8Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayUInt8", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<uint16_t> NoParamReturnArrayUInt16() {
		using NoParamReturnArrayUInt16Fn = std::vector<uint16_t> (*)();
		static NoParamReturnArrayUInt16Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayUInt16", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<uint32_t> NoParamReturnArrayUInt32() {
		using NoParamReturnArrayUInt32Fn = std::vector<uint32_t> (*)();
		static NoParamReturnArrayUInt32Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayUInt32", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<uint64_t> NoParamReturnArrayUInt64() {
		using NoParamReturnArrayUInt64Fn = std::vector<uint64_t> (*)();
		static NoParamReturnArrayUInt64Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayUInt64", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<void*> NoParamReturnArrayPtr64() {
		using NoParamReturnArrayPtr64Fn = std::vector<void*> (*)();
		static NoParamReturnArrayPtr64Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayPtr64", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<float> NoParamReturnArrayFloat() {
		using NoParamReturnArrayFloatFn = std::vector<float> (*)();
		static NoParamReturnArrayFloatFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayFloat", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<double> NoParamReturnArrayDouble() {
		using NoParamReturnArrayDoubleFn = std::vector<double> (*)();
		static NoParamReturnArrayDoubleFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayDouble", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline std::vector<std::string> NoParamReturnArrayString() {
		using NoParamReturnArrayStringFn = std::vector<std::string> (*)();
		static NoParamReturnArrayStringFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnArrayString", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline plugify::Vector2 NoParamReturnVector2() {
		using NoParamReturnVector2Fn = plugify::Vector2 (*)();
		static NoParamReturnVector2Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnVector2", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline plugify::Vector3 NoParamReturnVector3() {
		using NoParamReturnVector3Fn = plugify::Vector3 (*)();
		static NoParamReturnVector3Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnVector3", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline plugify::Vector4 NoParamReturnVector4() {
		using NoParamReturnVector4Fn = plugify::Vector4 (*)();
		static NoParamReturnVector4Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnVector4", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline plugify::Matrix4x4 NoParamReturnMatrix4x4() {
		using NoParamReturnMatrix4x4Fn = plugify::Matrix4x4 (*)();
		static NoParamReturnMatrix4x4Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.NoParamReturnMatrix4x4", reinterpret_cast<void**>(&__func));
		return __func();
	}
	inline void Param1(int32_t a) {
		using Param1Fn = void (*)(int32_t);
		static Param1Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.Param1", reinterpret_cast<void**>(&__func));
		__func(a);
	}
	inline void Param2(int32_t a, float b) {
		using Param2Fn = void (*)(int32_t, float);
		static Param2Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.Param2", reinterpret_cast<void**>(&__func));
		__func(a, b);
	}
	inline void Param3(int32_t a, float b, double c) {
		using Param3Fn = void (*)(int32_t, float, double);
		static Param3Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.Param3", reinterpret_cast<void**>(&__func));
		__func(a, b, c);
	}
	inline void Param4(int32_t a, float b, double c, const plugify::Vector4& d) {
		using Param4Fn = void (*)(int32_t, float, double, const plugify::Vector4&);
		static Param4Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.Param4", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d);
	}
	inline void Param5(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e) {
		using Param5Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&);
		static Param5Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.Param5", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e);
	}
	inline void Param6(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e, char16_t f) {
		using Param6Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&, char16_t);
		static Param6Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.Param6", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e, f);
	}
	inline void Param7(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e, char16_t f, const std::string& g) {
		using Param7Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&, char16_t, const std::string&);
		static Param7Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.Param7", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e, f, g);
	}
	inline void Param8(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e, char16_t f, const std::string& g, float h) {
		using Param8Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&, char16_t, const std::string&, float);
		static Param8Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.Param8", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e, f, g, h);
	}
	inline void Param9(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e, char16_t f, const std::string& g, float h, int16_t k) {
		using Param9Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&, char16_t, const std::string&, float, int16_t);
		static Param9Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.Param9", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e, f, g, h, k);
	}
	inline void Param10(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e, char16_t f, const std::string& g, float h, int16_t k, void* l) {
		using Param10Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&, char16_t, const std::string&, float, int16_t, void*);
		static Param10Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.Param10", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e, f, g, h, k, l);
	}
	inline void ParamRef1(int32_t& a) {
		using ParamRef1Fn = void (*)(int32_t&);
		static ParamRef1Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRef1", reinterpret_cast<void**>(&__func));
		__func(a);
	}
	inline void ParamRef2(int32_t& a, float& b) {
		using ParamRef2Fn = void (*)(int32_t&, float&);
		static ParamRef2Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRef2", reinterpret_cast<void**>(&__func));
		__func(a, b);
	}
	inline void ParamRef3(int32_t& a, float& b, double& c) {
		using ParamRef3Fn = void (*)(int32_t&, float&, double&);
		static ParamRef3Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRef3", reinterpret_cast<void**>(&__func));
		__func(a, b, c);
	}
	inline void ParamRef4(int32_t& a, float& b, double& c, plugify::Vector4& d) {
		using ParamRef4Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&);
		static ParamRef4Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRef4", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d);
	}
	inline void ParamRef5(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e) {
		using ParamRef5Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&);
		static ParamRef5Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRef5", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e);
	}
	inline void ParamRef6(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e, char16_t& f) {
		using ParamRef6Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&, char16_t&);
		static ParamRef6Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRef6", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e, f);
	}
	inline void ParamRef7(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e, char16_t& f, std::string& g) {
		using ParamRef7Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&, char16_t&, std::string&);
		static ParamRef7Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRef7", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e, f, g);
	}
	inline void ParamRef8(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e, char16_t& f, std::string& g, float& h) {
		using ParamRef8Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&, char16_t&, std::string&, float&);
		static ParamRef8Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRef8", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e, f, g, h);
	}
	inline void ParamRef9(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e, char16_t& f, std::string& g, float& h, int16_t& k) {
		using ParamRef9Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&, char16_t&, std::string&, float&, int16_t&);
		static ParamRef9Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRef9", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e, f, g, h, k);
	}
	inline void ParamRef10(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e, char16_t& f, std::string& g, float& h, int16_t& k, void*& l) {
		using ParamRef10Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&, char16_t&, std::string&, float&, int16_t&, void*&);
		static ParamRef10Fn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRef10", reinterpret_cast<void**>(&__func));
		__func(a, b, c, d, e, f, g, h, k, l);
	}
	inline void ParamRefVectors(std::vector<bool>& p1, std::vector<char16_t>& p2, std::vector<int8_t>& p3, std::vector<int16_t>& p4, std::vector<int32_t>& p5, std::vector<int64_t>& p6, std::vector<uint8_t>& p7, std::vector<uint16_t>& p8, std::vector<uint32_t>& p9, std::vector<uint64_t>& p10, std::vector<void*>& p11, std::vector<float>& p12, std::vector<double>& p13, std::vector<std::string>& p14) {
		using ParamRefVectorsFn = void (*)(std::vector<bool>&, std::vector<char16_t>&, std::vector<int8_t>&, std::vector<int16_t>&, std::vector<int32_t>&, std::vector<int64_t>&, std::vector<uint8_t>&, std::vector<uint16_t>&, std::vector<uint32_t>&, std::vector<uint64_t>&, std::vector<void*>&, std::vector<float>&, std::vector<double>&, std::vector<std::string>&);
		static ParamRefVectorsFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamRefVectors", reinterpret_cast<void**>(&__func));
		__func(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
	}
	inline int64_t ParamAllPrimitives(bool p1, char16_t p2, int8_t p3, int16_t p4, int32_t p5, int64_t p6, uint8_t p7, uint16_t p8, uint32_t p9, uint64_t p10, void* p11, float p12, double p13) {
		using ParamAllPrimitivesFn = int64_t (*)(bool, char16_t, int8_t, int16_t, int32_t, int64_t, uint8_t, uint16_t, uint32_t, uint64_t, void*, float, double);
		static ParamAllPrimitivesFn __func = nullptr;
		if (__func == nullptr) plugify::GetMethodPtr2("CSharpTest.ParamAllPrimitives", reinterpret_cast<void**>(&__func));
		return __func(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
	}
}
