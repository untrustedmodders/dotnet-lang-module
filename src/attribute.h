#pragma once

#include "core.h"

namespace netlm {
	class Type;

	class Attribute {
	public:
		Type& GetType();

		template<typename TReturn>
		TReturn GetFieldValue(std::string_view fieldName) {
			TReturn result;
			GetFieldValue(fieldName, &result);
			return result;
		}

		bool operator==(const Attribute& other) const { return _handle == other._handle; }

		operator bool() const { return _handle != -1; }

		ManagedHandle GetHandle() const { return _handle; }

	private:
		void GetFieldValue(std::string_view fieldName, void* valueVptr) const;

	private:
		ManagedHandle _handle{ -1 };
		std::unique_ptr<Type> _type;

		friend class Type;
		friend class MethodInfo;
		friend class FieldInfo;
		friend class PropertyInfo;
		friend class ManagedObject;
	};
}
