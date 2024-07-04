#pragma once

#include "core.h"
#include "strings.h"

namespace netlm {
	class Type;
	class Attribute;

	class PropertyInfo {
	public:
		std::string GetName() const;
		Type& GetType();

		std::vector<Attribute> GetAttributes() const;

	private:
		ManagedHandle _handle{ -1 };
		std::unique_ptr<Type> _type;

		friend class Type;
	};
}
