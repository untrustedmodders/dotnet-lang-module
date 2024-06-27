#pragma once

#include <plugify/method.h>
#include "managed_guid.h"
#include "managed_type.h"

namespace netlm {
	struct ManagedMethod {
		ManagedGuid guid;
		ManagedType returnType;
		std::vector<ManagedType> parameterTypes;
		std::vector<std::string> attributeNames;

		[[nodiscard]] bool HasAttribute(std::string_view attributeName) const {
			for (const std::string& name : attributeNames) {
				if (name == attributeName) {
					return true;
				}
			}

			return false;
		}
	};
}