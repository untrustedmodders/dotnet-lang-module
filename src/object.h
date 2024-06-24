#pragma once

#include "interop/managed_method.h"
#include "interop/managed_object.h"

namespace netlm {
	class Class;

	class Object {
	public:
		Object(Class* classPtr, ManagedObject managedObject);

		Object(const Object&) = delete;
		Object& operator=(const Object&) = delete;
		Object(Object&&) noexcept = delete;
		Object& operator=(Object&&) noexcept = delete;

		// Destructor frees the managed object
		~Object();

		[[nodiscard]] Class* GetClass() const { return _classPtr; }

		template<class ReturnType, class... Args>
		ReturnType InvokeMethod(const ManagedMethod* methodPtr, Args&&... args) const {
			static_assert(std::is_void_v<ReturnType> || std::is_trivial_v<ReturnType>, "Return type must be trivial to be used in interop");
			static_assert(std::is_void_v<ReturnType> || std::is_object_v<ReturnType>, "Return type must be either a value type or a pointer type to be used in interop (no references)");

			if constexpr (sizeof...(args) != 0) {
				void* argsVptr[] = {&args...};

				if constexpr (std::is_void_v<ReturnType>) {
					InvokeMethod(methodPtr, argsVptr, nullptr);
				} else {
					ReturnType returnValueStorage;
					InvokeMethod(methodPtr, argsVptr, &returnValueStorage);
					return returnValueStorage;
				}
			} else {
				if constexpr (std::is_void_v<ReturnType>) {
					InvokeMethod(methodPtr, nullptr, nullptr);
				} else {
					ReturnType returnValueStorage;
					InvokeMethod(methodPtr, nullptr, &returnValueStorage);
					return returnValueStorage;
				}
			}
		}

		template<class ReturnType, class... Args>
		 ReturnType InvokeMethodByName(const std::string& methodName, Args&&... args) const {
			const ManagedMethod* methodPtr = GetMethod(methodName);
			assert(methodPtr != nullptr && "Method not found");
			return InvokeMethod<ReturnType>(methodPtr, std::forward<Args>(args)...);
		}

	private:
		[[nodiscard]] const ManagedMethod* GetMethod(const std::string& methodName) const;
		void* InvokeMethod(const ManagedMethod* methodPtr, void** argsVptr, void* returnValueVptr) const;

		Class* _classPtr;
		ManagedObject _managedObject;
	};
}