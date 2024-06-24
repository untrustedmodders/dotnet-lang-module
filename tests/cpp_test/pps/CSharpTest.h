#pragma once

#include <plugify/cpp_plugin.h>
#include <vector>

//generated with https://github.com/untrustedmodders/cpp-lang-module/blob/main/generator/generator.py from CSharpTest 

namespace CSharpTest {
	inline void NoParamReturnVoid() {
		using NoParamReturnVoidFn = void (*)();
		static auto func = reinterpret_cast<NoParamReturnVoidFn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnVoid"));
		func();
	}
	inline bool NoParamReturnBool() {
		using NoParamReturnBoolFn = bool (*)();
		static auto func = reinterpret_cast<NoParamReturnBoolFn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnBool"));
		return func();
	}
	inline char16_t NoParamReturnChar16() {
		using NoParamReturnChar16Fn = char16_t (*)();
		static auto func = reinterpret_cast<NoParamReturnChar16Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnChar16"));
		return func();
	}
	inline int8_t NoParamReturnInt8() {
		using NoParamReturnInt8Fn = int8_t (*)();
		static auto func = reinterpret_cast<NoParamReturnInt8Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnInt8"));
		return func();
	}
	inline int16_t NoParamReturnInt16() {
		using NoParamReturnInt16Fn = int16_t (*)();
		static auto func = reinterpret_cast<NoParamReturnInt16Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnInt16"));
		return func();
	}
	inline int32_t NoParamReturnInt32() {
		using NoParamReturnInt32Fn = int32_t (*)();
		static auto func = reinterpret_cast<NoParamReturnInt32Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnInt32"));
		return func();
	}
	inline int64_t NoParamReturnInt64() {
		using NoParamReturnInt64Fn = int64_t (*)();
		static auto func = reinterpret_cast<NoParamReturnInt64Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnInt64"));
		return func();
	}
	inline uint8_t NoParamReturnUInt8() {
		using NoParamReturnUInt8Fn = uint8_t (*)();
		static auto func = reinterpret_cast<NoParamReturnUInt8Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnUInt8"));
		return func();
	}
	inline uint16_t NoParamReturnUInt16() {
		using NoParamReturnUInt16Fn = uint16_t (*)();
		static auto func = reinterpret_cast<NoParamReturnUInt16Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnUInt16"));
		return func();
	}
	inline uint32_t NoParamReturnUInt32() {
		using NoParamReturnUInt32Fn = uint32_t (*)();
		static auto func = reinterpret_cast<NoParamReturnUInt32Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnUInt32"));
		return func();
	}
	inline uint64_t NoParamReturnUInt64() {
		using NoParamReturnUInt64Fn = uint64_t (*)();
		static auto func = reinterpret_cast<NoParamReturnUInt64Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnUInt64"));
		return func();
	}
	inline void* NoParamReturnPtr64() {
		using NoParamReturnPtr64Fn = void* (*)();
		static auto func = reinterpret_cast<NoParamReturnPtr64Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnPtr64"));
		return func();
	}
	inline float NoParamReturnFloat() {
		using NoParamReturnFloatFn = float (*)();
		static auto func = reinterpret_cast<NoParamReturnFloatFn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnFloat"));
		return func();
	}
	inline double NoParamReturnDouble() {
		using NoParamReturnDoubleFn = double (*)();
		static auto func = reinterpret_cast<NoParamReturnDoubleFn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnDouble"));
		return func();
	}
	inline void* NoParamReturnFunction() {
		using NoParamReturnFunctionFn = void* (*)();
		static auto func = reinterpret_cast<NoParamReturnFunctionFn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnFunction"));
		return func();
	}
	inline std::string NoParamReturnString() {
		using NoParamReturnStringFn = std::string (*)();
		static auto func = reinterpret_cast<NoParamReturnStringFn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnString"));
		return func();
	}
	inline std::vector<bool> NoParamReturnArrayBool() {
		using NoParamReturnArrayBoolFn = std::vector<bool> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayBoolFn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayBool"));
		return func();
	}
	inline std::vector<char16_t> NoParamReturnArrayChar16() {
		using NoParamReturnArrayChar16Fn = std::vector<char16_t> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayChar16Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayChar16"));
		return func();
	}
	inline std::vector<int8_t> NoParamReturnArrayInt8() {
		using NoParamReturnArrayInt8Fn = std::vector<int8_t> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayInt8Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayInt8"));
		return func();
	}
	inline std::vector<int16_t> NoParamReturnArrayInt16() {
		using NoParamReturnArrayInt16Fn = std::vector<int16_t> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayInt16Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayInt16"));
		return func();
	}
	inline std::vector<int32_t> NoParamReturnArrayInt32() {
		using NoParamReturnArrayInt32Fn = std::vector<int32_t> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayInt32Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayInt32"));
		return func();
	}
	inline std::vector<int64_t> NoParamReturnArrayInt64() {
		using NoParamReturnArrayInt64Fn = std::vector<int64_t> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayInt64Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayInt64"));
		return func();
	}
	inline std::vector<uint8_t> NoParamReturnArrayUInt8() {
		using NoParamReturnArrayUInt8Fn = std::vector<uint8_t> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayUInt8Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayUInt8"));
		return func();
	}
	inline std::vector<uint16_t> NoParamReturnArrayUInt16() {
		using NoParamReturnArrayUInt16Fn = std::vector<uint16_t> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayUInt16Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayUInt16"));
		return func();
	}
	inline std::vector<uint32_t> NoParamReturnArrayUInt32() {
		using NoParamReturnArrayUInt32Fn = std::vector<uint32_t> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayUInt32Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayUInt32"));
		return func();
	}
	inline std::vector<uint64_t> NoParamReturnArrayUInt64() {
		using NoParamReturnArrayUInt64Fn = std::vector<uint64_t> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayUInt64Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayUInt64"));
		return func();
	}
	inline std::vector<void*> NoParamReturnArrayPtr64() {
		using NoParamReturnArrayPtr64Fn = std::vector<void*> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayPtr64Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayPtr64"));
		return func();
	}
	inline std::vector<float> NoParamReturnArrayFloat() {
		using NoParamReturnArrayFloatFn = std::vector<float> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayFloatFn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayFloat"));
		return func();
	}
	inline std::vector<double> NoParamReturnArrayDouble() {
		using NoParamReturnArrayDoubleFn = std::vector<double> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayDoubleFn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayDouble"));
		return func();
	}
	inline std::vector<std::string> NoParamReturnArrayString() {
		using NoParamReturnArrayStringFn = std::vector<std::string> (*)();
		static auto func = reinterpret_cast<NoParamReturnArrayStringFn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnArrayString"));
		return func();
	}
	inline plugify::Vector2 NoParamReturnVector2() {
		using NoParamReturnVector2Fn = plugify::Vector2 (*)();
		static auto func = reinterpret_cast<NoParamReturnVector2Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnVector2"));
		return func();
	}
	inline plugify::Vector3 NoParamReturnVector3() {
		using NoParamReturnVector3Fn = plugify::Vector3 (*)();
		static auto func = reinterpret_cast<NoParamReturnVector3Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnVector3"));
		return func();
	}
	inline plugify::Vector4 NoParamReturnVector4() {
		using NoParamReturnVector4Fn = plugify::Vector4 (*)();
		static auto func = reinterpret_cast<NoParamReturnVector4Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnVector4"));
		return func();
	}
	inline plugify::Matrix4x4 NoParamReturnMatrix4x4() {
		using NoParamReturnMatrix4x4Fn = plugify::Matrix4x4 (*)();
		static auto func = reinterpret_cast<NoParamReturnMatrix4x4Fn>(plugify::GetMethodPtr("CSharpTest.NoParamReturnMatrix4x4"));
		return func();
	}
	inline void Param1(int32_t a) {
		using Param1Fn = void (*)(int32_t);
		static auto func = reinterpret_cast<Param1Fn>(plugify::GetMethodPtr("CSharpTest.Param1"));
		func(a);
	}
	inline void Param2(int32_t a, float b) {
		using Param2Fn = void (*)(int32_t, float);
		static auto func = reinterpret_cast<Param2Fn>(plugify::GetMethodPtr("CSharpTest.Param2"));
		func(a, b);
	}
	inline void Param3(int32_t a, float b, double c) {
		using Param3Fn = void (*)(int32_t, float, double);
		static auto func = reinterpret_cast<Param3Fn>(plugify::GetMethodPtr("CSharpTest.Param3"));
		func(a, b, c);
	}
	inline void Param4(int32_t a, float b, double c, const plugify::Vector4& d) {
		using Param4Fn = void (*)(int32_t, float, double, const plugify::Vector4&);
		static auto func = reinterpret_cast<Param4Fn>(plugify::GetMethodPtr("CSharpTest.Param4"));
		func(a, b, c, d);
	}
	inline void Param5(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e) {
		using Param5Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&);
		static auto func = reinterpret_cast<Param5Fn>(plugify::GetMethodPtr("CSharpTest.Param5"));
		func(a, b, c, d, e);
	}
	inline void Param6(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e, char16_t f) {
		using Param6Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&, char16_t);
		static auto func = reinterpret_cast<Param6Fn>(plugify::GetMethodPtr("CSharpTest.Param6"));
		func(a, b, c, d, e, f);
	}
	inline void Param7(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e, char16_t f, const std::string& g) {
		using Param7Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&, char16_t, const std::string&);
		static auto func = reinterpret_cast<Param7Fn>(plugify::GetMethodPtr("CSharpTest.Param7"));
		func(a, b, c, d, e, f, g);
	}
	inline void Param8(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e, char16_t f, const std::string& g, float h) {
		using Param8Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&, char16_t, const std::string&, float);
		static auto func = reinterpret_cast<Param8Fn>(plugify::GetMethodPtr("CSharpTest.Param8"));
		func(a, b, c, d, e, f, g, h);
	}
	inline void Param9(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e, char16_t f, const std::string& g, float h, int16_t k) {
		using Param9Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&, char16_t, const std::string&, float, int16_t);
		static auto func = reinterpret_cast<Param9Fn>(plugify::GetMethodPtr("CSharpTest.Param9"));
		func(a, b, c, d, e, f, g, h, k);
	}
	inline void Param10(int32_t a, float b, double c, const plugify::Vector4& d, const std::vector<int64_t>& e, char16_t f, const std::string& g, float h, int16_t k, void* l) {
		using Param10Fn = void (*)(int32_t, float, double, const plugify::Vector4&, const std::vector<int64_t>&, char16_t, const std::string&, float, int16_t, void*);
		static auto func = reinterpret_cast<Param10Fn>(plugify::GetMethodPtr("CSharpTest.Param10"));
		func(a, b, c, d, e, f, g, h, k, l);
	}
	inline void ParamRef1(int32_t& a) {
		using ParamRef1Fn = void (*)(int32_t&);
		static auto func = reinterpret_cast<ParamRef1Fn>(plugify::GetMethodPtr("CSharpTest.ParamRef1"));
		func(a);
	}
	inline void ParamRef2(int32_t& a, float& b) {
		using ParamRef2Fn = void (*)(int32_t&, float&);
		static auto func = reinterpret_cast<ParamRef2Fn>(plugify::GetMethodPtr("CSharpTest.ParamRef2"));
		func(a, b);
	}
	inline void ParamRef3(int32_t& a, float& b, double& c) {
		using ParamRef3Fn = void (*)(int32_t&, float&, double&);
		static auto func = reinterpret_cast<ParamRef3Fn>(plugify::GetMethodPtr("CSharpTest.ParamRef3"));
		func(a, b, c);
	}
	inline void ParamRef4(int32_t& a, float& b, double& c, plugify::Vector4& d) {
		using ParamRef4Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&);
		static auto func = reinterpret_cast<ParamRef4Fn>(plugify::GetMethodPtr("CSharpTest.ParamRef4"));
		func(a, b, c, d);
	}
	inline void ParamRef5(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e) {
		using ParamRef5Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&);
		static auto func = reinterpret_cast<ParamRef5Fn>(plugify::GetMethodPtr("CSharpTest.ParamRef5"));
		func(a, b, c, d, e);
	}
	inline void ParamRef6(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e, char16_t& f) {
		using ParamRef6Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&, char16_t&);
		static auto func = reinterpret_cast<ParamRef6Fn>(plugify::GetMethodPtr("CSharpTest.ParamRef6"));
		func(a, b, c, d, e, f);
	}
	inline void ParamRef7(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e, char16_t& f, std::string& g) {
		using ParamRef7Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&, char16_t&, std::string&);
		static auto func = reinterpret_cast<ParamRef7Fn>(plugify::GetMethodPtr("CSharpTest.ParamRef7"));
		func(a, b, c, d, e, f, g);
	}
	inline void ParamRef8(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e, char16_t& f, std::string& g, float& h) {
		using ParamRef8Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&, char16_t&, std::string&, float&);
		static auto func = reinterpret_cast<ParamRef8Fn>(plugify::GetMethodPtr("CSharpTest.ParamRef8"));
		func(a, b, c, d, e, f, g, h);
	}
	inline void ParamRef9(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e, char16_t& f, std::string& g, float& h, int16_t& k) {
		using ParamRef9Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&, char16_t&, std::string&, float&, int16_t&);
		static auto func = reinterpret_cast<ParamRef9Fn>(plugify::GetMethodPtr("CSharpTest.ParamRef9"));
		func(a, b, c, d, e, f, g, h, k);
	}
	inline void ParamRef10(int32_t& a, float& b, double& c, plugify::Vector4& d, std::vector<int64_t>& e, char16_t& f, std::string& g, float& h, int16_t& k, void*& l) {
		using ParamRef10Fn = void (*)(int32_t&, float&, double&, plugify::Vector4&, std::vector<int64_t>&, char16_t&, std::string&, float&, int16_t&, void*&);
		static auto func = reinterpret_cast<ParamRef10Fn>(plugify::GetMethodPtr("CSharpTest.ParamRef10"));
		func(a, b, c, d, e, f, g, h, k, l);
	}
	inline void ParamRefVectors(std::vector<bool>& p1, std::vector<char16_t>& p2, std::vector<int8_t>& p3, std::vector<int16_t>& p4, std::vector<int32_t>& p5, std::vector<int64_t>& p6, std::vector<uint8_t>& p7, std::vector<uint16_t>& p8, std::vector<uint32_t>& p9, std::vector<uint64_t>& p10, std::vector<void*>& p11, std::vector<float>& p12, std::vector<double>& p13, std::vector<std::string>& p14) {
		using ParamRefVectorsFn = void (*)(std::vector<bool>&, std::vector<char16_t>&, std::vector<int8_t>&, std::vector<int16_t>&, std::vector<int32_t>&, std::vector<int64_t>&, std::vector<uint8_t>&, std::vector<uint16_t>&, std::vector<uint32_t>&, std::vector<uint64_t>&, std::vector<void*>&, std::vector<float>&, std::vector<double>&, std::vector<std::string>&);
		static auto func = reinterpret_cast<ParamRefVectorsFn>(plugify::GetMethodPtr("CSharpTest.ParamRefVectors"));
		func(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
	}
	inline int64_t ParamAllPrimitives(bool p1, char16_t p2, int8_t p3, int16_t p4, int32_t p5, int64_t p6, uint8_t p7, uint16_t p8, uint32_t p9, uint64_t p10, void* p11, float p12, double p13) {
		using ParamAllPrimitivesFn = int64_t (*)(bool, char16_t, int8_t, int16_t, int32_t, int64_t, uint8_t, uint16_t, uint32_t, uint64_t, void*, float, double);
		static auto func = reinterpret_cast<ParamAllPrimitivesFn>(plugify::GetMethodPtr("CSharpTest.ParamAllPrimitives"));
		return func(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
	}
}
