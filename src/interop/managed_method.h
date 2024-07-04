#pragma once

#include <plugify/method.h>
#include "managed_guid.h"
#include "managed_type.h"

namespace netlm {
	struct ManagedMethod {
		ManagedGuid guid;
		ManagedType returnType;
		std::vector<ManagedType> parameterTypes;
	};
}
