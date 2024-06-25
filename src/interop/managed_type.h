#pragma once

namespace netlm {

	struct ManagedParam;

	struct ManagedType {
		plugify::ValueType type;
		bool ref;
		/** Delegate info **/
		uint32_t paramCount;
		ManagedParam* paramTypes;
	};

	static_assert(sizeof(ManagedType) == 16, "ManagedType size mismatch with C#");

	struct ManagedParam {
		plugify::ValueType type;
		bool ref;

		bool operator==(const plugify::Property& property) const {
			return type == property.type && ref == property.ref;
		}
	};

	// Same as above but use ownership semantics.
	struct ManagedTypeHolder {
		plugify::ValueType type;
		bool ref;
		/** Delegate info **/
		uint32_t paramCount;
		std::unique_ptr<ManagedParam[]> paramTypes;

		ManagedTypeHolder() = default;
		explicit ManagedTypeHolder(ManagedType managedType) : type(managedType.type), ref(managedType.ref), paramCount(managedType.paramCount) {
			uint32_t size = paramCount + 1;
			paramTypes = std::make_unique<ManagedParam[]>(size);
			for (uint32_t i = 0; i < size; ++i) {
				paramTypes[i] = managedType.paramTypes[i];
			}
		}

		bool operator==(const plugify::Property& property) const {
			bool equal = type == property.type && ref == property.ref;
			if (!equal) 
				return false;
			
			if (paramCount == 0 && !property.prototype)
				return true;

			const auto& prototype = property.prototype;

			if (paramTypes[paramCount] != prototype->retType)
				return false;

			if (paramCount != prototype->paramTypes.size())
				return false;

			for (uint32_t i = 0; i < paramCount; ++i) {
				if (paramTypes[i] != prototype->paramTypes[i])
					return false;
			}
			
			return true;
		}
	};
}