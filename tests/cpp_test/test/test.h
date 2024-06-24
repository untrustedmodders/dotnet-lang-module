#pragma once

#include <vector>
#include <iostream>
#include <format>

struct Vector2 {
    float x, y;

    bool operator==(const Vector2&) const = default;
};

struct Vector3 {
    float x, y, z;

    bool operator==(const Vector3&) const = default;
};

struct Vector4 {
    float x, y, z, w;

    bool operator==(const Vector4&) const = default;
};

struct Matrix4x4 {
    float m00, m10, m20, m30;
    float m01, m11, m21, m31;
    float m02, m12, m22, m32;
    float m03, m13, m23, m33;

    bool operator==(const Matrix4x4&) const = default;
};


static Matrix4x4 CreateMatrix4x4(
        float m00, float m01, float m02, float m03,
        float m10, float m11, float m12, float m13,
        float m20, float m21, float m22, float m23,
        float m30, float m31, float m32, float m33)
{
    Matrix4x4 m;
    m.m00 = m00; m.m01 = m01; m.m02 = m02; m.m03 = m03;
    m.m10 = m10; m.m11 = m11; m.m12 = m12; m.m13 = m13;
    m.m20 = m20; m.m21 = m21; m.m22 = m22; m.m23 = m23;
    m.m30 = m30; m.m31 = m31; m.m32 = m32; m.m33 = m33;
    return m;
}

// C++ exported

// No Params, Only Return

extern "C" PLUGIN_API void NoParamReturnVoid()
{
    std::cout << "NoParamReturnVoid" << std::endl;
}
extern "C" PLUGIN_API bool NoParamReturnBool()
{
    std::cout << "NoParamReturnBool" << std::endl;
    return std::numeric_limits<bool>::max();
}
extern "C" PLUGIN_API char NoParamReturnChar8()
{
    std::cout << "NoParamReturnChar8" << std::endl;
    return std::numeric_limits<char>::max();
}
extern "C" PLUGIN_API char16_t NoParamReturnChar16()
{
    std::cout << "NoParamReturnChar16" << std::endl;
    return std::numeric_limits<char16_t>::max();
}
extern "C" PLUGIN_API int8_t NoParamReturnInt8()
{
    std::cout << "NoParamReturnInt8" << std::endl;
    return std::numeric_limits<int8_t>::max();
}
extern "C" PLUGIN_API int16_t NoParamReturnInt16()
{
    std::cout << "NoParamReturnInt16" << std::endl;
    return std::numeric_limits<int16_t>::max();
}
extern "C" PLUGIN_API int32_t NoParamReturnInt32()
{
    std::cout << "NoParamReturnInt32" << std::endl;
    return std::numeric_limits<int32_t>::max();
}
extern "C" PLUGIN_API int64_t NoParamReturnInt64()
{
    std::cout << "NoParamReturnInt64" << std::endl;
    return std::numeric_limits<int64_t>::max();
}
extern "C" PLUGIN_API uint8_t NoParamReturnUInt8()
{
    std::cout << "NoParamReturnUInt8" << std::endl;
    return std::numeric_limits<uint8_t>::max();
}
extern "C" PLUGIN_API uint16_t NoParamReturnUInt16()
{
    std::cout << "NoParamReturnUInt16" << std::endl;
    return std::numeric_limits<uint16_t>::max();
}
extern "C" PLUGIN_API uint32_t NoParamReturnUInt32()
{
    std::cout << "NoParamReturnUInt32" << std::endl;
    return std::numeric_limits<uint32_t>::max();
}
extern "C" PLUGIN_API uint64_t NoParamReturnUInt64()
{
    std::cout << "NoParamReturnUInt64" << std::endl;
    return std::numeric_limits<uint64_t>::max();
}
extern "C" PLUGIN_API void* NoParamReturnPtr64()
{
    std::cout << "NoParamReturnPtr64" << std::endl;
    return (void*)intptr_t(1);
}
extern "C" PLUGIN_API float NoParamReturnFloat()
{
    std::cout << "NoParamReturnFloat" << std::endl;
    return std::numeric_limits<float>::max();
}
extern "C" PLUGIN_API double NoParamReturnDouble()
{
    std::cout << "NoParamReturnDouble" << std::endl;
    return std::numeric_limits<double>::max();
}
extern "C" PLUGIN_API void* NoParamReturnFunction()
{
    std::cout << "NoParamReturnFunction" << std::endl;
    return nullptr;
}

// std::string
extern "C" PLUGIN_API void NoParamReturnString(std::string& output)
{
    std::cout << "NoParamReturnString" << std::endl;
    std::construct_at<>(&output, "Hello World");
}

// std::vector
extern "C" PLUGIN_API void NoParamReturnArrayBool(std::vector<bool>& output)
{
    std::cout << "NoParamReturnArrayBool" << std::endl;
    std::construct_at<>(&output, std::vector<bool>{ true, false });
}

extern "C" PLUGIN_API void NoParamReturnArrayChar8(std::vector<char>& output)
{
    std::cout << "NoParamReturnArrayChar8" << std::endl;
    std::construct_at<>(&output, std::vector<char>{ 'a', 'b', 'c' });
}

extern "C" PLUGIN_API void NoParamReturnArrayChar16(std::vector<char16_t>& output)
{
    std::cout << "NoParamReturnArrayChar16" << std::endl;
    std::construct_at<>(&output, std::vector<char16_t>{ 'a', 'b', 'c', 'd' });
}

extern "C" PLUGIN_API void NoParamReturnArrayInt8(std::vector<int8_t>& output)
{
    std::cout << "NoParamReturnArrayInt8" << std::endl;
    std::construct_at<>(&output, std::vector<int8_t>{ -3, -2, -1, 0, 1 });
}

extern "C" PLUGIN_API void NoParamReturnArrayInt16(std::vector<int16_t>& output)
{
    std::cout << "NoParamReturnArrayInt16" << std::endl;
    std::construct_at<>(&output, std::vector<int16_t>{ -4, -3, -2, -1, 0, 1 });
}

extern "C" PLUGIN_API void NoParamReturnArrayInt32(std::vector<int32_t>& output)
{
    std::cout << "NoParamReturnArrayInt32" << std::endl;
    std::construct_at<>(&output, std::vector<int32_t>{ -5, -4, -3, -2, -1, 0, 1 });
}

extern "C" PLUGIN_API void NoParamReturnArrayInt64(std::vector<int64_t>& output)
{
    std::cout << "NoParamReturnArrayInt64" << std::endl;
    std::construct_at<>(&output, std::vector<int64_t>{ -6, -5, -4, -3, -2, -1, 0, 1 });
}

extern "C" PLUGIN_API void NoParamReturnArrayUInt8(std::vector<uint8_t>& output)
{
    std::cout << "NoParamReturnArrayUInt8" << std::endl;
    std::construct_at<>(&output, std::vector<uint8_t>{ 0, 1, 2, 3, 4, 5, 6, 7, 8 });
}

extern "C" PLUGIN_API void NoParamReturnArrayUInt16(std::vector<uint16_t>& output)
{
    std::cout << "NoParamReturnArrayUInt16" << std::endl;
    std::construct_at<>(&output, std::vector<uint16_t>{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
}

extern "C" PLUGIN_API void NoParamReturnArrayUInt32(std::vector<uint32_t>& output)
{
    std::cout << "NoParamReturnArrayUInt32" << std::endl;
    std::construct_at<>(&output, std::vector<uint32_t>{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
}

extern "C" PLUGIN_API void NoParamReturnArrayUInt64(std::vector<uint64_t>& output)
{
    std::cout << "NoParamReturnArrayUInt64" << std::endl;
    std::construct_at<>(&output, std::vector<uint64_t>{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });
}

extern "C" PLUGIN_API void NoParamReturnArrayPtr64(std::vector<void*>& output)
{
    std::cout << "NoParamReturnArrayPtr64" << std::endl;
    std::construct_at<>(&output, std::vector<void*>{ (void*)intptr_t(0), (void*)intptr_t(1), (void*)intptr_t(2), (void*)intptr_t(3) });
}

extern "C" PLUGIN_API void NoParamReturnArrayFloat(std::vector<float>& output)
{
    std::cout << "NoParamReturnArrayFloat" << std::endl;
    std::construct_at<>(&output, std::vector<float>{ -12.34f, 0.0f, 12.34f });
}

extern "C" PLUGIN_API void NoParamReturnArrayDouble(std::vector<double>& output)
{
    std::cout << "NoParamReturnArrayDouble" << std::endl;
    std::construct_at<>(&output, std::vector<double>{ -12.345, 0.0, 12.345 });
}
extern "C" PLUGIN_API void NoParamReturnArrayString(std::vector<std::string>& output)
{
    std::cout << "NoParamReturnArrayString" << std::endl;
    std::construct_at<>(&output, std::vector<std::string>{ "1st string", "2nd string", "3rd element string (Should be big enough to avoid small string optimization)" });
}

// glm:vec
extern "C" PLUGIN_API Vector2 NoParamReturnVector2()
{
    std::cout << "NoParamReturnVector2" << std::endl;
    return Vector2(1, 2);
}
extern "C" PLUGIN_API Vector3 NoParamReturnVector3()
{
    std::cout << "NoParamReturnVector3" << std::endl;
    return Vector3(1, 2, 3);
}
extern "C" PLUGIN_API Vector4 NoParamReturnVector4()
{
    std::cout << "NoParamReturnVector4" << std::endl;
    return Vector4(1, 2, 3, 4);
}

//glm::mat
extern "C" PLUGIN_API Matrix4x4 NoParamReturnMatrix4x4()
{
    std::cout << "NoParamReturnMatrix4x4" << std::endl;
    return CreateMatrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
}

// Params (no refs)

extern "C" PLUGIN_API void Param1(int a)
{
    std::cout << std::format("Param1: a = \n", a);
}

extern "C" PLUGIN_API void Param2(int a, float b)
{
    std::cout << std::format("Param2: a = , b = \n", a, b);
}

extern "C" PLUGIN_API void Param3(int a, float b, double c)
{
    std::cout << std::format("Param3: a = , b = , c = \n", a, b, c);
}

extern "C" PLUGIN_API void Param4(int a, float b, double c, const Vector4& d)
{
    std::cout << std::format("Param4: a = , b = , c = , d = [,,,]\n", a, b, c, d.x, d.y, d.z, d.w);
}

extern "C" PLUGIN_API void Param5(int a, float b, double c, const Vector4& d, const std::vector<int64_t>& e)
{
    std::cout << std::format("Param5: a = , b = , c = , d = [,,,], e.size() = , e = [", a, b, c, d.x, d.y, d.z, d.w, e.size());
    for (const auto& elem : e) {
        std::cout << elem << ", ";
    }
    std::cout << "]\n";
}

extern "C" PLUGIN_API void Param6(int a, float b, double c, const Vector4& d, const std::vector<int64_t>& e, char f)
{
    std::cout << std::format("Param6: a = , b = , c = , d = [,,,], e.size() = , e = [", a, b, c, d.x, d.y, d.z, d.w, e.size());
    for (const auto& elem : e) {
        std::cout << elem << ", ";
    }
    std::cout << std::format("], f = \n", f);
}

extern "C" PLUGIN_API void Param7(int a, float b, double c, const Vector4& d, const std::vector<int64_t>& e, char f, const std::string& g)
{
    std::cout << std::format("Param7: a = , b = , c = , d = [,,,], e.size() = , e = [", a, b, c, d.x, d.y, d.z, d.w, e.size());
    for (const auto& elem : e) {
        std::cout << elem << ", ";
    }
    std::cout << std::format("], f = , g = \n", f, g);
}

extern "C" PLUGIN_API void Param8(int a, float b, double c, const Vector4& d, const std::vector<int64_t>& e, char f, const std::string& g, float h)
{
    std::cout << std::format("Param8: a = , b = , c = , d = [,,,], e.size() = , e = [", a, b, c, d.x, d.y, d.z, d.w, e.size());
    for (const auto& elem : e) {
        std::cout << elem << ", ";
    }
    std::cout << std::format("], f = , g = , h = \n", f, g, h);
}

extern "C" PLUGIN_API void Param9(int a, float b, double c, const Vector4& d, const std::vector<int64_t>& e, char f, const std::string& g, float h, int16_t k)
{
    std::cout << std::format("Param9: a = , b = , c = , d = [,,,], e.size() = , e = [", a, b, c, d.x, d.y, d.z, d.w, e.size());
    for (const auto& elem : e) {
        std::cout << elem << ", ";
    }
    std::cout << std::format("], f = , g = , h = , k = \n", f, g, h, k);
}

extern "C" PLUGIN_API void Param10(int a, float b, double c, const Vector4& d, const std::vector<int64_t>& e, char f, const std::string& g, float h, int16_t k, void* l)
{
    std::cout << std::format("Param10: a = , b = , c = , d = [,,,], e.size() = , e = [", a, b, c, d.x, d.y, d.z, d.w, e.size());
    for (const auto& elem : e) {
        std::cout << elem << ", ";
    }
    std::cout << std::format("], f = , g = , h = , k = , l = \n", f, g, h, k, l);
}

// Params (with refs)

extern "C" PLUGIN_API void ParamRef1(int& a)
{
    a = 42;
}

extern "C" PLUGIN_API void ParamRef2(int& a, float& b)
{
    a = 10;
    b = 3.14f;
}

extern "C" PLUGIN_API void ParamRef3(int& a, float& b, double& c)
{
    a = -20;
    b = 2.718f;
    c = 3.14159;
}

extern "C" PLUGIN_API void ParamRef4(int& a, float& b, double& c, Vector4& d)
{
    a = 100;
    b = -5.55f;
    c = 1.618;
    d = Vector4(1.0f, 2.0f, 3.0f, 4.0f);
}

extern "C" PLUGIN_API void ParamRef5(int& a, float& b, double& c, Vector4& d, std::vector<int64_t>& e)
{
    a = 500;
    b = -10.5f;
    c = 2.71828;
    d = Vector4(-1.0f, -2.0f, -3.0f, -4.0f);
    e = { -6, -5, -4, -3, -2, -1, 0, 1 };
}

extern "C" PLUGIN_API void ParamRef6(int& a, float& b, double& c, Vector4& d, std::vector<int64_t>& e, char& f)
{
    a = 750;
    b = 20.0f;
    c = 1.23456;
    d = Vector4(10.0f, 20.0f, 30.0f, 40.0f);
    e = { -6, -5, -4 };
    f = 'Z';
}

extern "C" PLUGIN_API void ParamRef7(int& a, float& b, double& c, Vector4& d, std::vector<int64_t>& e, char& f, std::string& g)
{
    a = -1000;
    b = 3.0f;
    c = -1.0;
    d = Vector4(100.0f, 200.0f, 300.0f, 400.0f);
    e = { -6, -5, -4, -3 };
    f = 'X';
    g = "Hello, World!";
}

extern "C" PLUGIN_API void ParamRef8(int& a, float& b, double& c, Vector4& d, std::vector<int64_t>& e, char& f, std::string& g, float& h)
{
    a = 999;
    b = -7.5f;
    c = 0.123456;
    d = Vector4(-100.0f, -200.0f, -300.0f, -400.0f);
    e = { -6, -5, -4, -3, -2, -1 };
    f = 'Y';
    g = "Goodbye, World!";
    h = 99.99f;
}

extern "C" PLUGIN_API void ParamRef9(int& a, float& b, double& c, Vector4& d, std::vector<int64_t>& e, char& f, std::string& g, float& h, int16_t& k)
{
    a = -1234;
    b = 123.45f;
    c = -678.9;
    d = Vector4(987.65f, 432.1f, 123.456f, 789.123f);
    e = { -6, -5, -4, -3, -2, -1, 0, 1, 5, 9 };
    f = 'A';
    g = "Testing, 1 2 3";
    h = -987.654f;
    k = 42;
}

extern "C" PLUGIN_API void ParamRef10(int& a, float& b, double& c, Vector4& d, std::vector<int64_t>& e, char& f, std::string& g, float& h, int16_t& k, void*& l)
{
    a = 987;
    b = -0.123f;
    c = 456.789;
    d = Vector4(-123.456f, 0.987f, 654.321f, -789.123f);
    e = { -6, -5, -4, -3, -2, -1, 0, 1, 5, 9, 4, -7 };
    f = 'B';
    g = "Another string";
    h = 3.141592f;
    k = -32768;
    l = nullptr;
}

// Params (array refs only)

extern "C" PLUGIN_API void ParamRefVectors(std::vector<bool>& p1, std::vector<char>& p2, std::vector<char16_t>& p3,
                                           std::vector<int8_t>& p4, std::vector<int16_t>& p5, std::vector<int32_t>& p6, std::vector<int64_t>& p7,
                                           std::vector<uint8_t>& p8, std::vector<uint16_t>& p9, std::vector<uint32_t>& p10, std::vector<uint64_t>& p11,
                                           std::vector<void*>& p12, std::vector<float>& p13, std::vector<double>& p14, std::vector<std::string>& p15)
{
    p1 = { true };
    p2 = { 'a', 'b', 'c' };
    p3 = { 'a', 'b', 'c' };
    p4 = { -3, -2, -1, 0, 1, 2, 3 };
    p5 = { -4, -3, -2, -1, 0, 1, 2, 3, 4 };
    p6 = { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5 };
    p7 = { -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6 };
    p8 = { 0, 1, 2, 3, 4, 5, 6, 7 };
    p9 = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    p10 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    p11 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    p12 = { nullptr, (void*)1 };
    p13 = { -12.34f, 0.0f, 12.34f };
    p14 = { -12.345, 0.0, 12.345 };
    p15 = { "Hello", "World", "OpenAI" };
}

// Params and Return (all primitive types)

extern "C" PLUGIN_API int64_t ParamAllPrimitives(bool p1, char16_t p2, int8_t p3, int16_t p4, int32_t p5, int64_t p6, uint8_t p7, uint16_t p8, uint32_t p9, uint64_t p10, void* p11, float p12, double p13)
{
    int64_t sum = 0;
    sum += static_cast<int64_t>(p1);
    sum += static_cast<int64_t>(p2);
    sum += static_cast<int64_t>(p3);
    sum += static_cast<int64_t>(p4);
    sum += static_cast<int64_t>(p5);
    sum += p6;
    sum += static_cast<int64_t>(p7);
    sum += static_cast<int64_t>(p8);
    sum += static_cast<int64_t>(p9);
    sum += static_cast<int64_t>(p10);
    sum += reinterpret_cast<int64_t>(p11); // Assumes pointer size is 64 bits
    sum += static_cast<int64_t>(p12);
    sum += static_cast<int64_t>(p13);
    return sum;
}	
