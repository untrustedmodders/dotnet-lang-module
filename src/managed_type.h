#pragma once

#include <plugify/method.h>

namespace netlm {
	struct ManagedType {
		plugify::ValueType type{};
		bool ref{};
	};

	static_assert(sizeof(ManagedType) == 2, "ManagedType size mismatch with C#");
}
