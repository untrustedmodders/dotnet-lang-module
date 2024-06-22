#pragma once

#include "managed_guid.h"

namespace netlm {
	struct ManagedMethod {
		ManagedGuid guid;
		std::vector<std::string> attribute_names;

		[[nodiscard]] bool HasAttribute(std::string_view attribute_name) const {
			for (const std::string& name : attribute_names) {
				if (name == attribute_name) {
					return true;
				}
			}

			return false;
		}
	};
}