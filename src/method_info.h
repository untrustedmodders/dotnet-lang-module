#pragma once

#include "core.h"
#include "native_string.h"

namespace netlm {
	class Type;
	class Attribute;

	class MethodInfo {
	public:
		std::string GetName() const;
		void* GetFunctionAddress() const;

		Type& GetReturnType();
		const std::vector<Type>& GetParameterTypes();

		TypeAccessibility GetAccessibility() const;

		std::vector<Attribute> GetAttributes() const;
		std::vector<Attribute> GetParameterAttributes(size_t index) const;
		std::vector<Attribute> GetReturnAttributes() const;

		bool operator==(const MethodInfo& other) const { return _handle == other._handle; }
		operator bool() const { return _handle != -1; }
		ManagedHandle GetHandle() const { return _handle; }

	private:
		ManagedHandle _handle = -1;
		std::unique_ptr<Type> _returnType;
		std::vector<Type> _parameterTypes;

		friend class Type;
		friend class ManagedObject;
	};
}
