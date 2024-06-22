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

#include <filesystem>
namespace fs = std::filesystem;

#include <plugify/compat_format.h>

namespace std {
	template<typename T>
	using deleted_unique_ptr = std::unique_ptr<T,std::function<void(T*)>>;
}

namespace netlm {
	template <class K, class V>
	using HashMap = std::unordered_map<K, V>;

	template <class T>
	using UniquePtr = std::unique_ptr<T>;

	template <class T>
	using SharedPtr = std::shared_ptr<T>;

	using String = std::string;
}