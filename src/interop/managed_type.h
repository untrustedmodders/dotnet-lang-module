#pragma once

namespace netlm {

	struct ManagedType {
		plugify::ValueType type{};
		bool ref{false};
	};

	static_assert(sizeof(ManagedType) == 2, "ManagedType size mismatch with C#");
}
