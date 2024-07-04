#pragma once

#include "core.h"
#include "strings.h"

namespace netlm {
	class Type;
	class Attribute;

	class MethodInfo {
	public:
		std::string GetName() const;

		Type& GetReturnType();
		const std::vector<std::unique_ptr<Type>>& GetParameterTypes();

		TypeAccessibility GetAccessibility() const;

		std::vector<Attribute> GetAttributes() const;

	private:
		ManagedHandle _handle{ -1 };
		std::unique_ptr<Type> _returnType;
		std::vector<std::unique_ptr<Type>> _parameterTypes;

		friend class Type;
	};
}
