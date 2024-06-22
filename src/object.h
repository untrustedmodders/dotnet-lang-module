#pragma once

#include "interop/managed_method.h"
#include "interop/managed_object.h"

namespace netlm {
	class Class;

	class Object {
	public:
		Object(Class* class_ptr, ManagedObject managed_object);

		Object(const Object&) = delete;
		Object& operator=(const Object&) = delete;
		Object(Object&&) noexcept = delete;
		Object& operator=(Object&&) noexcept = delete;

		// Destructor frees the managed object
		~Object();

		Class* GetClass() const { return m_class_ptr; }

		template<class ReturnType, class... Args>
		ReturnType InvokeMethod(const ManagedMethod* method_ptr, Args&&... args) {
			static_assert(std::is_void_v<ReturnType> || std::is_trivial_v<ReturnType>, "Return type must be trivial to be used in interop");
			static_assert(std::is_void_v<ReturnType> || std::is_object_v<ReturnType>, "Return type must be either a value type or a pointer type to be used in interop (no references)");

			if constexpr (sizeof...(args) != 0) {
				void* args_vptr[] = {&args...};

				if constexpr (std::is_void_v<ReturnType>) {
					InvokeMethod(method_ptr, args_vptr, nullptr);
				} else {
					ReturnType return_value_storage;
					InvokeMethod(method_ptr, args_vptr, &return_value_storage);
					return return_value_storage;
				}
			} else {
				if constexpr (std::is_void_v<ReturnType>) {
					InvokeMethod(method_ptr, nullptr, nullptr);
				} else {
					ReturnType return_value_storage;
					InvokeMethod(method_ptr, nullptr, &return_value_storage);
					return return_value_storage;
				}
			}
		}

		template<class ReturnType, class... Args>
		 ReturnType InvokeMethodByName(const String& method_name, Args&&... args) {
			const ManagedMethod* method_ptr = GetMethod(method_name);
			//AssertThrowMsg(method_ptr != nullptr, "Method %s not found", method_name.data());
			return InvokeMethod<ReturnType>(method_ptr, std::forward<Args>(args)...);
		}

	private:
		const ManagedMethod* GetMethod(const String& method_name) const;
		void* InvokeMethod(const ManagedMethod* method_ptr, void** args_vptr, void* return_value_vptr);

		Class* m_class_ptr;
		ManagedObject m_managed_object;
	};

}