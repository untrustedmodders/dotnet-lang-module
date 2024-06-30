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
		int32_t typeHash;
		Class* classObject;
		ManagedGuid assemblyGuid;
		ManagedGuid newObjectGuid;
		ManagedGuid freeObjectGuid;
	};

	using MethodMap = std::unordered_map<std::string, ManagedMethod>;

	class Class {
	public:
		using NewObjectFunction = ManagedObject (*)();
		using FreeObjectFunction = void (*)(ManagedObject);

		Class(ClassHolder* parent, std::string name)
			: _parent{parent},
			  _name{std::move(name)},
			  _newObjectFunction{nullptr},
			  _freeObjectFunction{nullptr} {
		}

		Class(const Class&) = delete;
		Class& operator=(const Class&) = delete;
		Class(Class&&) noexcept = delete;
		Class& operator=(Class&&) noexcept = delete;
		~Class() = default;

		[[nodiscard]] const std::string& GetName() const { return _name; }
		[[nodiscard]] ClassHolder* GetParent() const { return _parent; }

		[[nodiscard]] NewObjectFunction GetNewObjectFunction() const { return _newObjectFunction; }
		void SetNewObjectFunction(NewObjectFunction newObjectFptr) { _newObjectFunction = newObjectFptr; }

		[[nodiscard]] FreeObjectFunction GetFreeObjectFunction() const { return _freeObjectFunction; }
		void SetFreeObjectFunction(FreeObjectFunction freeObjectFptr) { _freeObjectFunction = freeObjectFptr; }

		[[nodiscard]] bool HasMethod(const std::string& methodName) const { return _methods.find(methodName) != _methods.end(); }

		[[nodiscard]] bool IsAssignableFrom(std::string_view typeHash) const;
		void SetBaseClasses(char** baseClasses, uint32_t numBaseClasses) { _baseClasses.assign(baseClasses, baseClasses + numBaseClasses); }

		[[nodiscard]] ManagedMethod* GetMethod(const std::string& methodName);
		[[nodiscard]] const ManagedMethod* GetMethod(const std::string& methodName) const;

		[[nodiscard]] const MethodMap& GetMethods() const { return _methods; }

		void AddMethod(const std::string& methodName, ManagedMethod&& methodObject) { _methods[methodName] = std::move(methodObject); }

		void EnsureLoaded() const;

		std::unique_ptr<Object> NewObject();

		template<class ReturnType, class... Args>
		ReturnType InvokeStaticMethod(const std::string& methodName, Args&&... args) const {
			static_assert(std::is_void_v<ReturnType> || std::is_trivial_v<ReturnType>, "Return type must be trivial to be used in interop");
			static_assert(std::is_void_v<ReturnType> || std::is_object_v<ReturnType>, "Return type must be either a value type or a pointer type to be used in interop (no references)");

			auto it = _methods.find(methodName);
			assert(it != _methods.end() && "Method not found");

			const auto& methodObject = std::get<ManagedMethod>(*it);

			void* argsVptr[] = { &args... };

			if constexpr (std::is_void_v<ReturnType>) {
				InvokeStaticMethod(&methodObject, argsVptr, nullptr);
			} else {
				ReturnType returnValueStorage;
				void* resultVptr = InvokeStaticMethod(&methodObject, argsVptr, &returnValueStorage);
				return returnValueStorage;
			}
		}

		void* InvokeStaticMethod(const ManagedMethod* methodPtr, void** argsVptr, void* returnValueVptr) const;

	private:
		std::string _name;

		MethodMap _methods;
		std::vector<std::string> _baseClasses;

		ClassHolder* _parent;

		NewObjectFunction _newObjectFunction;
		FreeObjectFunction _freeObjectFunction;
	};

}