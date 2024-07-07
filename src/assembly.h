#pragma once

#include <string_view>
#include <string>
#include <filesystem>
#include <memory>

namespace netlm {
	class Assembly {
	public:
		static std::unique_ptr<Assembly> LoadFromPath(const fs::path& assemblyPath);
		static std::string GetError();

		Assembly(const Assembly&) = delete;
		Assembly& operator=(const Assembly&) = delete;
		Assembly(Assembly&&) noexcept = delete;
		Assembly& operator=(Assembly&&) noexcept = delete;
		~Assembly();

		void* GetFunction(const char* functionName) const;
		template<class TFunc> requires(std::is_pointer_v<TFunc> && std::is_function_v<std::remove_pointer_t<TFunc>>)
		TFunc GetFunction(const char* functionName) const {
			return reinterpret_cast<TFunc>(GetFunction(functionName));
		}

		bool operator==(const Assembly& other) const { return _handle == other._handle; }
		operator bool() const { return _handle != nullptr; }
		//void* GetHandle() const { return _handle; }

	private:
		explicit Assembly(void* handle);

	private:
		void* _handle = nullptr;
	};
}
