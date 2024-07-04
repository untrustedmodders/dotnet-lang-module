#pragma once

#include "core.h"
#include "strings.h"

namespace netlm {
	class Type;
	class Attribute;

	class FieldInfo {
	public:
		std::string GetName() const;
		Type& GetType();

		TypeAccessibility GetAccessibility() const;

		std::vector<Attribute> GetAttributes() const;

	private:
		ManagedHandle _handle{ -1 };
		std::unique_ptr<Type> _type;

		friend class Type;
	};
}
