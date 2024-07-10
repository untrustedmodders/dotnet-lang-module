cmake_minimum_required(VERSION 3.14 FATAL_ERROR)

if(POLICY CMP0092)
    cmake_policy(SET CMP0092 NEW) # Don't add -W3 warning level by default.
endif()


project(dotnet-lang-module VERSION 1.0.0.0 DESCRIPTION "C# (.NET) Language Module" HOMEPAGE_URL "https://github.com/untrustedmodders/dotnet-lang-module" LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)
set(CMAKE_C_STANDARD 11)
set(CMAKE_C_STANDARD_REQUIRED ON)
set(CMAKE_C_EXTENSIONS OFF)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

if(NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE Debug)
endif()

if(UNIX AND NOT APPLE)
    set(LINUX TRUE)
endif()

#
# CPPtrace
#
include(cmake/cpptrace.cmake)

#
# Dyncall
#
include(cmake/dyncall.cmake)

#
# Plugify
#
set(PLUGIFY_BUILD_SHARED_LIB ON CACHE INTERNAL "")
set(PLUGIFY_BUILD_FUNCTION ON CACHE INTERNAL "")
set(PLUGIFY_BUILD_ASSEMBLY ON CACHE INTERNAL "")
if(LINUX)
    set(PLUGIFY_USE_STATIC_STDLIB ON CACHE INTERNAL "")
endif()
add_subdirectory(external/plugify)

#
# C# Language Module for Plugify
#
file(GLOB_RECURSE NETLM_SOURCES RELATIVE ${CMAKE_CURRENT_SOURCE_DIR} "src/*.cpp")
set(NETLM_PCH_FILE "src/pch.h")

add_library(${PROJECT_NAME} SHARED ${NETLM_SOURCES})

set(NETLM_LINK_LIBRARIES plugify::plugify plugify::plugify-function plugify::plugify-assembly asmjit::asmjit cpptrace::cpptrace dyncall_s)

if(NOT COMPILER_SUPPORTS_FORMAT)
    set(NETLM_LINK_LIBRARIES ${NETLM_LINK_LIBRARIES} fmt::fmt-header-only)
endif()

include_directories(${dyncall_SOURCE_DIR})
include_directories(external/dotnet/include)

target_link_libraries(${PROJECT_NAME} PRIVATE ${NETLM_LINK_LIBRARIES})
target_precompile_headers(${PROJECT_NAME} PRIVATE ${NETLM_PCH_FILE})

if(MSVC)
    target_compile_options(${PROJECT_NAME} PRIVATE /W4 /WX /Wv:18)
else()
    target_compile_options(${PROJECT_NAME} PRIVATE -Wextra -Wconversion -Werror) # -Wshadow -Wpedantic
endif()

if(APPLE)
    target_link_options(${PROJECT_NAME} PRIVATE "-Wl,-exported_symbols_list,${CMAKE_CURRENT_SOURCE_DIR}/sym/exported_symbols.lds")
elseif(UNIX)
    target_link_options(${PROJECT_NAME} PRIVATE "-Wl,--version-script,${CMAKE_CURRENT_SOURCE_DIR}/sym/version_script.lds")
endif()

include(GenerateExportHeader)
generate_export_header(${PROJECT_NAME} EXPORT_MACRO_NAME NETLM_EXPORT EXPORT_FILE_NAME ${CMAKE_BINARY_DIR}/exports/module_export.h)
target_include_directories(${PROJECT_NAME} PRIVATE ${CMAKE_BINARY_DIR}/exports)

target_compile_definitions(${PROJECT_NAME} PRIVATE
        NETLM_PLATFORM_WINDOWS=$<BOOL:${WIN32}>
        NETLM_PLATFORM_APPLE=$<BOOL:${APPLE}>
        NETLM_PLATFORM_LINUX=$<BOOL:${LINUX}>
        NETLM_LIBRARY_SUFFIX="${CMAKE_SHARED_LIBRARY_SUFFIX}"
        NETLM_LIBRARY_PREFIX="${CMAKE_SHARED_LIBRARY_PREFIX}"
)

set(NETLM_VERSION "0" CACHE STRING "Set version name")
set(NETLM_PACKAGE "${PROJECT_NAME}" CACHE STRING "Set package name")

configure_file(
    ${CMAKE_SOURCE_DIR}/${PROJECT_NAME}.pmodule.in
    ${CMAKE_BINARY_DIR}/${PROJECT_NAME}.pmodule
)