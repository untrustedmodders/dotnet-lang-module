#pragma once

#include "core.h"
#include "native_string.h"

namespace netlm {
	class Type;
	class Attribute;

	class FieldInfo {
	public:
		std::string GetName() const;
		Type& GetType();

		TypeAccessibility GetAccessibility() const;

		std::vector<Attribute> GetAttributes() const;

		bool operator==(const FieldInfo& other) const { return _handle == other._handle; }
		operator bool() const { return _handle; }
		ManagedHandle GetHandle() const { return _handle; }

	private:
		ManagedHandle _handle{};
		std::unique_ptr<Type> _type;

		friend class Type;
		friend class ManagedObject;
	};
}
