#pragma once

#include "interop/managed_method.h"
#include "interop/managed_object.h"
#include "interop/managed_guid.h"

namespace netlm {
	class Object;
	class Class;
	class ClassHolder;
	class Assembly;

	struct ManagedClass {
		int32_t type_hash;
		Class* class_object;
		ManagedGuid assembly_guid;
		ManagedGuid new_object_guid;
		ManagedGuid free_object_guid;
	};
}

namespace netlm {
	namespace detail {
		template<class T>
		constexpr inline T* ToPointerType(T* value) { return value; }
		template<class T>
		constexpr inline T* ToPointerType(T** value) { return *value; }
	}

	class Class {
	public:
		// Function to create a new object of this class with the given arguments
		using NewObjectFunction = ManagedObject (*)(void);
		using FreeObjectFunction = void (*)(ManagedObject);

		Class(ClassHolder* parent, String name)
			: m_parent(parent),
			  m_name(std::move(name)),
			  m_new_object_fptr(nullptr) {
		}

		Class(const Class&) = delete;
		Class& operator=(const Class&) = delete;
		Class(Class&&) noexcept = delete;
		Class& operator=(Class&&) noexcept = delete;
		~Class() = default;

		const String& GetName() const { return m_name; }
		ClassHolder* GetParent() const { return m_parent; }
		NewObjectFunction GetNewObjectFunction() const { return m_new_object_fptr; }
		void SetNewObjectFunction(NewObjectFunction new_object_fptr) { m_new_object_fptr = new_object_fptr; }

		FreeObjectFunction GetFreeObjectFunction() const { return m_free_object_fptr; }

		void SetFreeObjectFunction(FreeObjectFunction free_object_fptr) { m_free_object_fptr = free_object_fptr; }

		/*! \brief Check if a method exists by name.
		 *
		 *  \param method_name The name of the method to check.
		 *
		 *  \return True if the method exists, otherwise false.
		 */
		bool HasMethod(const String& method_name) const { return m_methods.find(method_name) != m_methods.end(); }

		/*! \brief Get a method by name.
		 *
		 *  \param method_name The name of the method to get.
		 *
		 *  \return A pointer to the method object if it exists, otherwise nullptr.
		 */
		ManagedMethod* GetMethod(const String& method_name) {
			auto it = m_methods.find(method_name);
			if (it == m_methods.end()) {
				return nullptr;
			}

			return &it->second;
		}

		/*! \brief Get a method by name.
		 *
		 *  \param method_name The name of the method to get.
		 *
		 *  \return A pointer to the method object if it exists, otherwise nullptr.
		 */
		const ManagedMethod* GetMethod(const String& method_name) const {
			auto it = m_methods.find(method_name);
			if (it == m_methods.end()) {
				return nullptr;
			}

			return &it->second;
		}

		/*! \brief Add a method to this class.
		 *
		 *  \param method_name The name of the method to add.
		 *  \param method_object The method object to add.
		 */
		void AddMethod(const String& method_name, ManagedMethod&& method_object) { m_methods[method_name] = std::move(method_object); }

		/*! \brief Get all methods of this class.
		 *
		 *  \return A reference to the map of methods.
		 */
		const HashMap<String, ManagedMethod>& GetMethods() const { return m_methods; }

		void EnsureLoaded() const;

		/*! \brief Create a new managed object of this class.
		 *     The new object will be managed by the .NET runtime and will be freed when the unique pointer goes out of scope.
		 *      The returned object will hold a reference to this class instance, so it will need to remain valid for the lifetime of the object.
		 *
		 *  \return A unique pointer to the new managed object.
		 */
		UniquePtr<Object> NewObject();

		template<class ReturnType, class... Args>
		ReturnType InvokeStaticMethod(const String& method_name, Args&&... args) {
			static_assert(std::is_void_v<ReturnType> || std::is_trivial_v<ReturnType>, "Return type must be trivial to be used in interop");
			static_assert(std::is_void_v<ReturnType> || std::is_object_v<ReturnType>, "Return type must be either a value type or a pointer type to be used in interop (no references)");

			auto it = m_methods.find(method_name);
			//AssertThrowMsg(it != m_methods.end(), "Method not found");

			const ManagedMethod& method_object = it->second;

			void* args_vptr[] = {&args...};

			if constexpr (std::is_void_v<ReturnType>) {
				InvokeStaticMethod(&method_object, args_vptr, nullptr);
			} else {
				ReturnType return_value_storage;
				void* result_vptr = InvokeStaticMethod(&method_object, args_vptr, &return_value_storage);
				return return_value_storage;
			}
		}

	private:
		void* InvokeStaticMethod(const ManagedMethod* method_ptr, void** args_vptr, void* return_value_vptr);

		String m_name;
		HashMap<String, ManagedMethod> m_methods;

		ClassHolder* m_parent;

		NewObjectFunction m_new_object_fptr;
		FreeObjectFunction m_free_object_fptr;
	};

}// namespace netlm