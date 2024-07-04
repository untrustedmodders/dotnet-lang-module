#pragma once

#include <string_view>
#include <string>
#include <filesystem>
#include <memory>

namespace netlm {
	class Library {
	public:
		static std::unique_ptr<Library> LoadFromPath(const std::filesystem::path& assemblyPath);
		static std::string GetError();

		Library(const Library&) = delete;
		Library& operator=(const Library&) = delete;
		Library(Library&&) noexcept = delete;
		Library& operator=(Library&&) noexcept = delete;
		~Library();

		void* GetFunction(const char* functionName) const;
		template<class F> requires(std::is_pointer_v<F> && std::is_function_v<std::remove_pointer_t<F>>)
		F GetFunction(const char* functionName) const {
			return reinterpret_cast<F>(GetFunction(functionName));
		}

	private:
		explicit Library(void* handle);

	private:
		void* _handle{ nullptr };
	};
}
