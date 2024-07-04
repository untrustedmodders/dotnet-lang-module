#pragma once

#include "core.h"

namespace netlm {
	class Type;

	class Attribute {
	public:
		Type& GetType();

		template<typename ReturnType>
		ReturnType GetFieldValue(std::string_view fieldName) {
			ReturnType result;
			GetFieldValue(fieldName, &result);
			return result;
		}

	private:
		void GetFieldValue(std::string_view fieldName, void* valueVptr) const;

	private:
		ManagedHandle _handle{ -1 };
		std::unique_ptr<Type> _type;

		friend class Type;
		friend class MethodInfo;
		friend class FieldInfo;
		friend class PropertyInfo;
	};
}
