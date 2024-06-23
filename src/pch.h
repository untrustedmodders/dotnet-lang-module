#pragma once

#include <string>
#include <vector>
#include <map>
#include <set>
#include <unordered_map>
#include <unordered_set>
#include <cstdint>
#include <limits>
#include <utility>
#include <functional>
#include <optional>
#include <span>
#include <mutex>
#include <cassert>

#include <filesystem>
namespace fs = std::filesystem;

#include <plugify/compat_format.h>

namespace std {
	template<typename T>
	using deleted_unique_ptr = std::unique_ptr<T, std::function<void(T*)>>;
}