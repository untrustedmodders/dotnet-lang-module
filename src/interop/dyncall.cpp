#include <module_export.h>
#include <dyncall/dyncall.h>

// Dyncall
NETLM_EXPORT DCCallVM* NewVM(size_t size) { return dcNewCallVM(size); }
NETLM_EXPORT void Free(DCCallVM* vm) { dcFree(vm); }
NETLM_EXPORT void Reset(DCCallVM* vm) { dcReset(vm); }
NETLM_EXPORT void Mode(DCCallVM* vm, int mode) { dcMode(vm, mode); }

NETLM_EXPORT void ArgBool(DCCallVM* vm, bool value) { dcArgBool(vm, value); }
NETLM_EXPORT void ArgChar(DCCallVM* vm, char value) { dcArgChar(vm, value); }
NETLM_EXPORT void ArgInt8(DCCallVM* vm, int8_t value) { dcArgChar(vm, value); }
NETLM_EXPORT void ArgUInt8(DCCallVM* vm, uint8_t value) { dcArgChar(vm, static_cast<int8_t>(value)); }
NETLM_EXPORT void ArgInt16(DCCallVM* vm, int16_t value) { dcArgShort(vm, value); }
NETLM_EXPORT void ArgUInt16(DCCallVM* vm, uint16_t value) { dcArgShort(vm, static_cast<int16_t>(value)); }
NETLM_EXPORT void ArgInt32(DCCallVM* vm, int32_t value) { dcArgInt(vm, value); }
NETLM_EXPORT void ArgUInt32(DCCallVM* vm, uint32_t value) { dcArgInt(vm, static_cast<int32_t>(value)); }
NETLM_EXPORT void ArgInt64(DCCallVM* vm, int64_t value) { dcArgLongLong(vm, value); }
NETLM_EXPORT void ArgUInt64(DCCallVM* vm, uint64_t value) { dcArgLongLong(vm, static_cast<int64_t>(value)); }
NETLM_EXPORT void ArgFloat(DCCallVM* vm, float value) { dcArgFloat(vm, value); }
NETLM_EXPORT void ArgDouble(DCCallVM* vm, double value) { dcArgDouble(vm, value); }
NETLM_EXPORT void ArgPointer(DCCallVM* vm, void* value) { dcArgPointer(vm, value); }
NETLM_EXPORT void ArgAggr(DCCallVM* vm, DCaggr* ag, void* value) { dcArgAggr(vm, ag, value); }

NETLM_EXPORT void CallVoid(DCCallVM* vm, void* funcptr) { dcCallVoid(vm, funcptr); }
NETLM_EXPORT bool CallBool(DCCallVM* vm, void* funcptr) { return dcCallBool(vm, funcptr); }
NETLM_EXPORT char CallChar(DCCallVM* vm, void* funcptr) { return dcCallChar(vm, funcptr); }
NETLM_EXPORT int8_t CallInt8(DCCallVM* vm, void* funcptr) { return dcCallChar(vm, funcptr); }
NETLM_EXPORT uint8_t CallUInt8(DCCallVM* vm, void* funcptr) { return static_cast<uint8_t>(dcCallChar(vm, funcptr)); }
NETLM_EXPORT int16_t CallInt16(DCCallVM* vm, void* funcptr) { return dcCallShort(vm, funcptr); }
NETLM_EXPORT uint16_t CallUInt16(DCCallVM* vm, void* funcptr) { return static_cast<uint16_t>(dcCallShort(vm, funcptr)); }
NETLM_EXPORT int32_t CallInt32(DCCallVM* vm, void* funcptr) { return dcCallInt(vm, funcptr); }
NETLM_EXPORT uint32_t CallUInt32(DCCallVM* vm, void* funcptr) { return static_cast<uint32_t>(dcCallInt(vm, funcptr)); }
NETLM_EXPORT int64_t CallInt64(DCCallVM* vm, void* funcptr) { return dcCallLongLong(vm, funcptr); }
NETLM_EXPORT uint64_t CallUInt64(DCCallVM* vm, void* funcptr) { return static_cast<uint64_t>(dcCallLongLong(vm, funcptr)); }
NETLM_EXPORT float CallFloat(DCCallVM* vm, void* funcptr) { return dcCallFloat(vm, funcptr); }
NETLM_EXPORT double CallDouble(DCCallVM* vm, void* funcptr) { return dcCallDouble(vm, funcptr); }
NETLM_EXPORT void* CallPointer(DCCallVM* vm, void* funcptr) { return dcCallPointer(vm, funcptr); }
NETLM_EXPORT void CallAggr(DCCallVM* vm, void* funcptr, DCaggr* ag, void* returnValue) { dcCallAggr(vm, funcptr, ag, returnValue); }

NETLM_EXPORT int GetError(DCCallVM* vm) { return dcGetError(vm); }

NETLM_EXPORT DCaggr* NewAggr(size_t fieldCount, size_t size) { return dcNewAggr(fieldCount, size); }
NETLM_EXPORT void AggrField(DCaggr* ag, char type, int offset, size_t arrayLength) { dcAggrField(ag, type, offset, arrayLength); }
NETLM_EXPORT void CloseAggr(DCaggr* ag) { dcCloseAggr(ag); }
NETLM_EXPORT void FreeAggr(DCaggr* ag) { dcFreeAggr(ag); }

NETLM_EXPORT int GetModeFromCCSigChar(char sigChar) { return dcGetModeFromCCSigChar(sigChar); }