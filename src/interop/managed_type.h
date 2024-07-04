#pragma once

namespace netlm {

	struct ManagedType {
		int32_t typeHash{ -1 };
		plugify::ValueType type{};
		bool ref{ false };
	};

	static_assert(sizeof(ManagedType) == 8, "ManagedType size mismatch with C#");
}
