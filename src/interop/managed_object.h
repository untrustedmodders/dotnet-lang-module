#pragma once

#include "managed_guid.h"

namespace netlm {
	struct ManagedObject {
		ManagedGuid guid;
		void* ptr{nullptr};
	};

	static_assert(sizeof(ManagedObject) == 24, "ManagedObject size mismatch with C#");
}
