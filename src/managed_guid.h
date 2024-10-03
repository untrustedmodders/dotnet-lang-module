#pragma once

#include <type_traits>

namespace netlm {
	struct ManagedGuid {
		uint64_t low;
		uint64_t high;

		bool operator==(const ManagedGuid&) const = default;
		operator bool() const { return low || high; }
	};

	static_assert(sizeof(ManagedGuid) == 16, "ManagedGuid size mismatch with C#");
	static_assert(std::is_standard_layout_v<ManagedGuid>, "ManagedGuid is not standard layout");
}

namespace std {
	template <>
	struct hash<netlm::ManagedGuid> {
		size_t operator()(netlm::ManagedGuid guid) const noexcept {
			size_t h1 = hash<uint64_t>{}(guid.low);
			size_t h2 = hash<uint64_t>{}(guid.high);

			return h1 ^ (h2 + 0x9e3779b9 + (h1 << 6) + (h1 >> 2));
		}
	};
}