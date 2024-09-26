#!/usr/bin/python3
import sys
import argparse
import os
import json
from enum import Enum

TYPES_MAP = {
    'void': 'void',
    'bool': 'bool',
    'char8': 'char',
    'char16': 'char',
    'int8': 'sbyte',
    'int16': 'short',
    'int32': 'int',
    'int64': 'long',
    'uint8': 'byte',
    'uint16': 'ushort',
    'uint32': 'uint',
    'uint64': 'ulong',
    'ptr64': 'nint',
    'float': 'float',
    'double': 'double',
    'function': 'delegate',
    'string': 'string',
    'bool*': 'bool[]',
    'char8*': 'char[]',
    'char16*': 'char[]',
    'int8*': 'sbyte[]',
    'int16*': 'short[]',
    'int32*': 'int[]',
    'int64*': 'long[]',
    'uint8*': 'byte[]',
    'uint16*': 'ushort[]',
    'uint32*': 'uint[]',
    'uint64*': 'ulong[]',
    'ptr64*': 'nint[]',
    'float*': 'float[]',
    'double*': 'double[]',
    'string*': 'string[]',
    'vec2': 'Vector2',
    'vec3': 'Vector3',
    'vec4': 'Vector4',
    'mat4x4': 'Matrix4x4'
}

WTYPES_MAP = {
    'void': 'void',
    'bool': 'bool',
    'char8': 'char',
    'char16': 'char',
    'int8': 'sbyte',
    'int16': 'short',
    'int32': 'int',
    'int64': 'long',
    'uint8': 'byte',
    'uint16': 'ushort',
    'uint32': 'uint',
    'uint64': 'ulong',
    'ptr64': 'nint',
    'float': 'float',
    'double': 'double',
    'function': 'nint',
    'string': 'nint',
    'bool*': 'nint',
    'char8*': 'nint',
    'char16*': 'nint',
    'int8*': 'nint',
    'int16*': 'nint',
    'int32*': 'nint',
    'int64*': 'nint',
    'uint8*': 'nint',
    'uint16*': 'nint',
    'uint32*': 'nint',
    'uint64*': 'nint',
    'ptr64*': 'nint',
    'float*': 'nint',
    'double*': 'nint',
    'string*': 'nint',
    'vec2': 'Vector2',
    'vec3': 'Vector3',
    'vec4': 'Vector4',
    'mat4x4': 'Matrix4x4'
}

CTYPES_MAP = {
    'void': 'void',
    'bool': 'bool',
    'char8': 'sbyte',
    'char16': 'ushort',
    'int8': 'sbyte',
    'int16': 'short',
    'int32': 'int',
    'int64': 'long',
    'uint8': 'byte',
    'uint16': 'ushort',
    'uint32': 'uint',
    'uint64': 'ulong',
    'ptr64': 'nint',
    'float': 'float',
    'double': 'double',
    'function': 'nint',
    'string': '*',
    'bool*': '*',
    'char8*': '*',
    'char16*': '*',
    'int8*': '*',
    'int16*': '*',
    'int32*': '*',
    'int64*': '*',
    'uint8*': '*',
    'uint16*': '*',
    'uint32*': '*',
    'uint64*': '*',
    'ptr64*': '*',
    'float*': '*',
    'double*': '*',
    'string*': '*',
    'vec2': 'Vector2*',
    'vec3': 'Vector3*',
    'vec4': 'Vector4*',
    'mat4x4': 'Matrix4x4*'
}

VAL_TYPESCAST_MAP = {
    'void': '',
    'bool': '',
    'char8': 'Convert.ToSByte',
    'char16': 'Convert.ToUInt16',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': 'NativeMethods.CreateString',
    'bool*': 'NativeMethods.CreateVectorBool',
    'char8*': 'NativeMethods.CreateVectorChar8',
    'char16*': 'NativeMethods.CreateVectorChar16',
    'int8*': 'NativeMethods.CreateVectorInt8',
    'int16*': 'NativeMethods.CreateVectorInt16',
    'int32*': 'NativeMethods.CreateVectorInt32',
    'int64*': 'NativeMethods.CreateVectorInt64',
    'uint8*': 'NativeMethods.CreateVectorUInt8',
    'uint16*': 'NativeMethods.CreateVectorUInt16',
    'uint32*': 'NativeMethods.CreateVectorUInt32',
    'uint64*': 'NativeMethods.CreateVectorUInt64',
    'ptr64*': 'NativeMethods.CreateVectorIntPtr',
    'float*': 'NativeMethods.CreateVectorFloat',
    'double*': 'NativeMethods.CreateVectorDouble',
    'string*': 'NativeMethods.CreateVectorString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

RET_TYPESCAST_MAP = {
    'void': '',
    'bool': '',
    'char8': 'Convert.ToSByte',
    'char16': 'Convert.ToUInt16',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': 'NativeMethods.AllocateString',
    'bool*': 'NativeMethods.AllocateVectorBool',
    'char8*': 'NativeMethods.AllocateVectorChar8',
    'char16*': 'NativeMethods.AllocateVectorChar16',
    'int8*': 'NativeMethods.AllocateVectorInt8',
    'int16*': 'NativeMethods.AllocateVectorInt16',
    'int32*': 'NativeMethods.AllocateVectorInt32',
    'int64*': 'NativeMethods.AllocateVectorInt64',
    'uint8*': 'NativeMethods.AllocateVectorUInt8',
    'uint16*': 'NativeMethods.AllocateVectorUInt16',
    'uint32*': 'NativeMethods.AllocateVectorUInt32',
    'uint64*': 'NativeMethods.AllocateVectorUInt64',
    'ptr64*': 'NativeMethods.AllocateVectorIntPtr',
    'float*': 'NativeMethods.AllocateVectorFloat',
    'double*': 'NativeMethods.AllocateVectorDouble',
    'string*': 'NativeMethods.AllocateVectorString',
    'vec2': 'Vector2',
    'vec3': 'Vector3',
    'vec4': 'Vector4',
    'mat4x4': 'Matrix4x4'
}

ASS_TYPESCAST_MAP = {
    'void': '',
    'bool': '',
    'char8': 'Convert.ToChar',
    'char16': 'Convert.ToChar',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': 'NativeMethods.GetStringData',
    'bool*': 'NativeMethods.GetVectorDataBool',
    'char8*': 'NativeMethods.GetVectorDataChar8',
    'char16*': 'NativeMethods.GetVectorDataChar16',
    'int8*': 'NativeMethods.GetVectorDataInt8',
    'int16*': 'NativeMethods.GetVectorDataInt16',
    'int32*': 'NativeMethods.GetVectorDataInt32',
    'int64*': 'NativeMethods.GetVectorDataInt64',
    'uint8*': 'NativeMethods.GetVectorDataUInt8',
    'uint16*': 'NativeMethods.GetVectorDataUInt16',
    'uint32*': 'NativeMethods.GetVectorDataUInt32',
    'uint64*': 'NativeMethods.GetVectorDataUInt64',
    'ptr64*': 'NativeMethods.GetVectorDataIntPtr',
    'float*': 'NativeMethods.GetVectorDataFloat',
    'double*': 'NativeMethods.GetVectorDataDouble',
    'string*': 'NativeMethods.GetVectorDataString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

SIZ_TYPESCAST_MAP = {
    'void': '',
    'bool': '',
    'char8': '',
    'char16': '',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': '',
    'bool*': 'NativeMethods.GetVectorSizeBool',
    'char8*': 'NativeMethods.GetVectorSizeChar8',
    'char16*': 'NativeMethods.GetVectorSizeChar16',
    'int8*': 'NativeMethods.GetVectorSizeInt8',
    'int16*': 'NativeMethods.GetVectorSizeInt16',
    'int32*': 'NativeMethods.GetVectorSizeInt32',
    'int64*': 'NativeMethods.GetVectorSizeInt64',
    'uint8*': 'NativeMethods.GetVectorSizeUInt8',
    'uint16*': 'NativeMethods.GetVectorSizeUInt16',
    'uint32*': 'NativeMethods.GetVectorSizeUInt32',
    'uint64*': 'NativeMethods.GetVectorSizeUInt64',
    'ptr64*': 'NativeMethods.GetVectorSizeIntPtr',
    'float*': 'NativeMethods.GetVectorSizeFloat',
    'double*': 'NativeMethods.GetVectorSizeDouble',
    'string*': 'NativeMethods.GetVectorSizeString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

DEL_TYPESCAST_MAP = {
    'void': '',
    'bool': '',
    'char8': '',
    'char16': '',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': 'NativeMethods.DeleteString',
    'bool*': 'NativeMethods.DeleteVectorBool',
    'char8*': 'NativeMethods.DeleteVectorChar8',
    'char16*': 'NativeMethods.DeleteVectorChar16',
    'int8*': 'NativeMethods.DeleteVectorInt8',
    'int16*': 'NativeMethods.DeleteVectorInt16',
    'int32*': 'NativeMethods.DeleteVectorInt32',
    'int64*': 'NativeMethods.DeleteVectorInt64',
    'uint8*': 'NativeMethods.DeleteVectorUInt8',
    'uint16*': 'NativeMethods.DeleteVectorUInt16',
    'uint32*': 'NativeMethods.DeleteVectorUInt32',
    'uint64*': 'NativeMethods.DeleteVectorUInt64',
    'ptr64*': 'NativeMethods.DeleteVectorIntPtr',
    'float*': 'NativeMethods.DeleteVectorFloat',
    'double*': 'NativeMethods.DeleteVectorDouble',
    'string*': 'NativeMethods.DeleteVectorString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

FRE_TYPESCAST_MAP = {
    'void': '',
    'bool': '',
    'char8': '',
    'char16': '',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': 'NativeMethods.FreeString',
    'bool*': 'NativeMethods.FreeVectorBool',
    'char8*': 'NativeMethods.FreeVectorChar8',
    'char16*': 'NativeMethods.FreeVectorChar16',
    'int8*': 'NativeMethods.FreeVectorInt8',
    'int16*': 'NativeMethods.FreeVectorInt16',
    'int32*': 'NativeMethods.FreeVectorInt32',
    'int64*': 'NativeMethods.FreeVectorInt64',
    'uint8*': 'NativeMethods.FreeVectorUInt8',
    'uint16*': 'NativeMethods.FreeVectorUInt16',
    'uint32*': 'NativeMethods.FreeVectorUInt32',
    'uint64*': 'NativeMethods.FreeVectorUInt64',
    'ptr64*': 'NativeMethods.FreeVectorIntPtr',
    'float*': 'NativeMethods.FreeVectorFloat',
    'double*': 'NativeMethods.FreeVectorDouble',
    'string*': 'NativeMethods.FreeVectorString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

DAT_WRAPPER_MAP = {
    'void': '',
    'bool': '',
    'char8': '',
    'char16': '',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': 'NativeMethods.GetStringData',
    'bool*': 'NativeMethods.GetVectorDataBool',
    'char8*': 'NativeMethods.GetVectorDataChar8',
    'char16*': 'NativeMethods.GetVectorDataChar16',
    'int8*': 'NativeMethods.GetVectorDataInt8',
    'int16*': 'NativeMethods.GetVectorDataInt16',
    'int32*': 'NativeMethods.GetVectorDataInt32',
    'int64*': 'NativeMethods.GetVectorDataInt64',
    'uint8*': 'NativeMethods.GetVectorDataUInt8',
    'uint16*': 'NativeMethods.GetVectorDataUInt16',
    'uint32*': 'NativeMethods.GetVectorDataUInt32',
    'uint64*': 'NativeMethods.GetVectorDataUInt64',
    'ptr64*': 'NativeMethods.GetVectorDataIntPtr',
    'float*': 'NativeMethods.GetVectorDataFloat',
    'double*': 'NativeMethods.GetVectorDataDouble',
    'string*': 'NativeMethods.GetVectorDataString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

SIZ_WRAPPER_MAP = {
    'void': '',
    'bool': '',
    'char8': '',
    'char16': '',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': '',
    'bool*': 'bool[NativeMethods.GetVectorSizeBool',
    'char8*': 'char[NativeMethods.GetVectorSizeChar8',
    'char16*': 'char[NativeMethods.GetVectorSizeChar16',
    'int8*': 'sbyte[NativeMethods.GetVectorSizeInt8',
    'int16*': 'short[NativeMethods.GetVectorSizeInt16',
    'int32*': 'int[NativeMethods.GetVectorSizeInt32',
    'int64*': 'long[NativeMethods.GetVectorSizeInt64',
    'uint8*': 'byte[NativeMethods.GetVectorSizeUInt8',
    'uint16*': 'ushort[NativeMethods.GetVectorSizeUInt16',
    'uint32*': 'uint[NativeMethods.GetVectorSizeUInt32',
    'uint64*': 'ulong[NativeMethods.GetVectorSizeUInt64',
    'ptr64*': 'nint[NativeMethods.GetVectorSizeIntPtr',
    'float*': 'float[NativeMethods.GetVectorSizeFloat',
    'double*': 'double[NativeMethods.GetVectorSizeDouble',
    'string*': 'string[NativeMethods.GetVectorSizeString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

RET_WRAPPER_MAP = {
    'void': '',
    'bool': '',
    'char8': '',
    'char16': '',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': 'NativeMethods.AllocateString',
    'bool*': 'NativeMethods.AllocateVectorBool',
    'char8*': 'NativeMethods.AllocateVectorChar8',
    'char16*': 'NativeMethods.AllocateVectorChar16',
    'int8*': 'NativeMethods.AllocateVectorInt8',
    'int16*': 'NativeMethods.AllocateVectorInt16',
    'int32*': 'NativeMethods.AllocateVectorInt32',
    'int64*': 'NativeMethods.AllocateVectorInt64',
    'uint8*': 'NativeMethods.AllocateVectorUInt8',
    'uint16*': 'NativeMethods.AllocateVectorUInt16',
    'uint32*': 'NativeMethods.AllocateVectorUInt32',
    'uint64*': 'NativeMethods.AllocateVectorUInt64',
    'ptr64*': 'NativeMethods.AllocateVectorIntPtr',
    'float*': 'NativeMethods.AllocateVectorFloat',
    'double*': 'NativeMethods.AllocateVectorDouble',
    'string*': 'NativeMethods.AllocateVectorString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

ASS_WRAPPER_MAP = {
    'void': '',
    'bool': '',
    'char8': '',
    'char16': '',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': 'NativeMethods.AssignString',
    'bool*': 'NativeMethods.AssignVectorDataBool',
    'char8*': 'NativeMethods.AssignVectorDataChar8',
    'char16*': 'NativeMethods.AssignVectorDataChar16',
    'int8*': 'NativeMethods.AssignVectorDataInt8',
    'int16*': 'NativeMethods.AssignVectorDataInt16',
    'int32*': 'NativeMethods.AssignVectorDataInt32',
    'int64*': 'NativeMethods.AssignVectorDataInt64',
    'uint8*': 'NativeMethods.AssignVectorDataUInt8',
    'uint16*': 'NativeMethods.AssignVectorDataUInt16',
    'uint32*': 'NativeMethods.AssignVectorDataUInt32',
    'uint64*': 'NativeMethods.AssignVectorDataUInt64',
    'ptr64*': 'NativeMethods.AssignVectorDataIntPtr',
    'float*': 'NativeMethods.AssignVectorDataFloat',
    'double*': 'NativeMethods.AssignVectorDataDouble',
    'string*': 'NativeMethods.AssignVectorDataString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

CTR_WRAPPER_MAP = {
    'void': '',
    'bool': '',
    'char8': '',
    'char16': '',
    'int8': '',
    'int16': '',
    'int32': '',
    'int64': '',
    'uint8': '',
    'uint16': '',
    'uint32': '',
    'uint64': '',
    'ptr64': '',
    'float': '',
    'double': '',
    'function': '',
    'string': 'NativeMethods.ConstructString',
    'bool*': 'NativeMethods.ConstructVectorDataBool',
    'char8*': 'NativeMethods.ConstructVectorDataChar8',
    'char16*': 'NativeMethods.ConstructVectorDataChar16',
    'int8*': 'NativeMethods.ConstructVectorDataInt8',
    'int16*': 'NativeMethods.ConstructVectorDataInt16',
    'int32*': 'NativeMethods.ConstructVectorDataInt32',
    'int64*': 'NativeMethods.ConstructVectorDataInt64',
    'uint8*': 'NativeMethods.ConstructVectorDataUInt8',
    'uint16*': 'NativeMethods.ConstructVectorDataUInt16',
    'uint32*': 'NativeMethods.ConstructVectorDataUInt32',
    'uint64*': 'NativeMethods.ConstructVectorDataUInt64',
    'ptr64*': 'NativeMethods.ConstructVectorDataIntPtr',
    'float*': 'NativeMethods.ConstructVectorDataFloat',
    'double*': 'NativeMethods.ConstructVectorDataDouble',
    'string*': 'NativeMethods.ConstructVectorDataString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

INVALID_NAMES = {
    'abstract',
    'as',
    'base',
    'bool',
    'break',
    'byte',
    'case',
    'catch',
    'char',
    'checked',
    'class',
    'const',
    'continue',
    'decimal',
    'default',
    'delegate',
    'do',
    'double',
    'else',
    'enum',
    'event',
    'explicit',
    'extern',
    'false',
    'finally',
    'fixed',
    'float',
    'for',
    'foreach',
    'goto',
    'if',
    'implicit',
    'in',
    'int',
    'interface',
    'internal',
    'is',
    'lock',
    'long',
    'namespace',
    'new',
    'null',
    'object',
    'operator',
    'out',
    'override',
    'params',
    'private',
    'protected',
    'internal',
    'readonly',
    'ref',
    'return',
    'sbyte',
    'sealed',
    'short',
    'sizeof',
    'stackalloc',
    'static',
    'string',
    'struct',
    'switch',
    'this',
    'throw',
    'true',
    'try',
    'typeof',
    'uint',
    'ulong',
    'unchecked',
    'unsafe',
    'ushort',
    'using',
    'virtual',
    'void',
    'volatile',
    'while'
    #'add',
    #'and',
    #'alias',
    #'ascending',
    #'args',
    #'async',
    #'await',
    #'by',
    #'descending',
    #'dynamic',
    #'equals',
    #'file',
    #'from',
    #'get',
    #'global',
    #'group',
    #'init',
    #'into',
    #'join',
    #'let',
    #'managed',
    #'nameof',
    #'nint',
    #'not',
    #'notnull',
    #'nuint',
    #'on',
    #'or',
    #'orderby',
    #'partial',
    #'partial',
    #'record',
    #'remove',
    #'required',
    #'scoped',
    #'select',
    #'set',
    #'unmanaged',
    #'value',
    #'var',
    #'when',
    #'where',
    #'where',
    #'with',
    #'yield'
}

def validate_manifest(pplugin):
    parse_errors = []
    methods = pplugin.get('exportedMethods')
    if type(methods) is list:
        for i, method in enumerate(methods):
            if type(method) is dict:
                if type(method.get('type')) is str:
                    parse_errors += [f'root.exportedMethods[{i}].type not string']
            else:
                parse_errors += [f'root.exportedMethods[{i}] not object']
    else:
        parse_errors += ['root.exportedMethods not array']
    return parse_errors


def convert_type(type_name, is_ref=False):
    type = TYPES_MAP.get(type_name, 'int')
    if is_ref:
        return 'ref ' + type
    else:
        return type

def convert_wtype(type_name, is_ref=False):
    type = WTYPES_MAP.get(type_name, 'int')
    if is_ref:
        return 'ref ' + type
    else:
        return type


def convert_ctype(type_name, is_ref=False, is_ret=False):
    type = CTYPES_MAP.get(type_name, 'int')
    if is_ref:
        if type == '*':
            return 'nint'
        elif '*' in type:
            return type[:-1] + '*'
        else:
            return type + '*'
    else:
        if type == '*':
            return 'nint'
        elif is_ret and '*' in type:
            return type[:-1]
        else:
            return type


def is_obj_return(type_name):
    return '*' in type_name or type_name == 'string'


def is_need_marshal(method):
    if is_obj_return(method['retType']['type']):
        return True
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        for p in it:
            if is_obj_return(p['type']):
                return True
    return False


def generate_name(name):
    if name in INVALID_NAMES:
        return name + '_'
    else:
        return name


class ParamGen(Enum):
    Types = 1
    Names = 2
    TypesNames = 3
    CastNames = 4
    WrapperNames = 5
    WrapperTypes = 6
    WrapperCastNames = 7


def gen_delegate(prototype):
    ret_type = prototype['retType']
    return_type = convert_type(ret_type['type'], 'ref' in ret_type and ret_type['ref'] is True)
    return (f'\tdelegate {return_type} '
            f'{prototype["name"]}({gen_params_string(prototype, ParamGen.TypesNames)});\n')


def gen_wrapper_delegate(prototype):
    ret_type = prototype['retType']
    return_type = convert_wtype(ret_type['type'], 'ref' in ret_type and ret_type['ref'] is True)
    if is_obj_return(ret_type['type']):
        return (f'\tdelegate nint '
                f'{prototype["name"]}Wrapper({return_type} __output, {gen_params_string(prototype, ParamGen.WrapperTypes)});\n')
    else:
        return (f'\tdelegate {return_type} '
                f'{prototype["name"]}Wrapper({gen_params_string(prototype, ParamGen.WrapperTypes)});\n')


def gen_params_string(method, param_gen: ParamGen):
    def gen_param(param):
        if param_gen == ParamGen.Types:
            type = convert_type(param['type'], 'ref' in param and param['ref'] is True)
            if param['type'] == 'function':
                type = generate_name(param['prototype']['name'])
            return type
        elif param_gen == ParamGen.Names:
            return generate_name(param['name'])
        elif param_gen == ParamGen.WrapperCastNames:
            if is_obj_return(param['type']):
                if 'ref' in param and param['ref'] is True:
                    return 'ref __' + generate_name(param['name']) + '__'
                else:
                    return '__' + generate_name(param['name']) + '__'
            else:
                return '@__' + generate_name(param['name'])
        elif param_gen == ParamGen.CastNames:
            if param['type'] == 'function':
                if is_need_marshal(param['prototype']):
                    return f'Marshal.GetFunctionPointerForDelegate({method["name"]}_{generate_name(param["name"])})'
                else:
                    return f'Marshal.GetFunctionPointerForDelegate({generate_name(param["name"])})'
            elif is_obj_return(param['type']):
                return '__' + generate_name(param['name'])
            elif param['type'] == 'char8' or param['type'] == 'char16':
                if 'ref' in param and param['ref'] is True:
                    return '&__' + generate_name(param['name'])
                else:
                    return '__' + generate_name(param['name'])
            elif 'vec' in param['type'] or 'mat' in param['type']:
                if 'ref' in param and param['ref'] is True:
                    return '__' + generate_name(param['name'])
                else:
                    return '&' + generate_name(param['name'])
            else:
                if 'ref' in param and param['ref'] is True:
                    return '__' + generate_name(param['name'])
                else:
                    return generate_name(param['name'])
        elif param_gen == ParamGen.WrapperNames:
            type = convert_wtype(param['type'], 'ref' in param and param['ref'] is True)
            if 'delegate' in type and 'prototype' in param:
                type = generate_name(param['prototype']['name'])
            return f'{type} @__{generate_name(param["name"])}'
        elif param_gen == ParamGen.WrapperTypes:
            type = convert_wtype(param['type'], 'ref' in param and param['ref'] is True)
            if 'delegate' in type and 'prototype' in param:
                type = generate_name(param['prototype']['name'])
            return f'{type} {generate_name(param["name"])}'
        type = convert_type(param['type'], 'ref' in param and param['ref'] is True)
        if 'delegate' in type and 'prototype' in param:
            type = generate_name(param['prototype']['name'])
        return f'{type} {generate_name(param["name"])}'

    def gen_return(param):
        if param_gen == ParamGen.WrapperCastNames:
            return '__output__'
        elif param_gen == ParamGen.WrapperNames:
            return 'nint @__output'
        else:
            return '__output'

    output_string = ''
    ret_type = method['retType']
    c_conv = param_gen == ParamGen.CastNames or param_gen == ParamGen.WrapperNames
    is_obj_ret = is_obj_return(ret_type['type']) and c_conv
    if is_obj_ret:
        output_string += f'{gen_return(ret_type)}'
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        if not is_obj_ret:
            output_string += gen_param(next(it))
        for p in it:
            output_string += f', {gen_param(p)}'
    return output_string


def gen_ctypes_string(method):
    output_string = ''
    ret_type = method['retType']
    obj_return = is_obj_return(ret_type['type'])
    if obj_return:
        output_string += f'{convert_ctype(ret_type["type"], True)}'
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        if not obj_return:
            param = next(it)
            output_string += convert_ctype(param['type'], 'ref' in param and param['ref'] is True)
        for p in it:
            output_string += f', {convert_ctype(p["type"], "ref" in p and p["ref"] is True)}'
    if output_string != '':
        output_string += ', '
    if obj_return:
        output_string += 'void'
    else:
        output_string += f'{convert_ctype(ret_type["type"], is_ret=True)}'
    return output_string


def gen_types_string(method):
    def gen_param(param):
        type = convert_type(param['type'], 'ref' in param and param['ref'] is True)
        if 'delegate' in type and 'prototype' in param:
            type = generate_name(param['prototype']['name'])
        return type

    output_string = ''
    ret_type = method['retType']
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        param = next(it)
        output_string += gen_param(param)
        for p in it:
            output_string += f', {gen_param(p)}'
    if output_string != '':
        output_string += ', '
    output_string += f'{gen_param(ret_type)}'
    return output_string


def gen_paramscast_string(method):
    def gen_param(param):
        type = VAL_TYPESCAST_MAP.get(param['type'], 'int')
        name = generate_name(param['name'])
        if 'CreateVector' in type:
            return f'var __{name} = {type}({name}, {name}.Length)'
        elif type != '':
            return f'var __{name} = {type}({name})'
        else:
            if 'ref' in param and param['ref'] is True:
                ctype = TYPES_MAP.get(param['type'], 'int')
                return f'fixed({ctype}* __{name} = &{name}) {{'
            else:
                return ''

    def gen_return(param):
        type = RET_TYPESCAST_MAP.get(param['type'], 'int')
        return f'var __output = {type}()'

    output_string = ''
    ret_type = method['retType']
    is_obj_ret = is_obj_return(ret_type['type'])
    if is_obj_ret:
        ret = gen_return(ret_type)
        if ret != '':
            output_string += f'\t\t\t{ret};\n'
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        param = gen_param(next(it))
        if param != '':
            output_string += f'\t\t\t{param}'
            if output_string[-1] != '{':
                output_string += ';\n'
            else:
                output_string += '\n'
        for p in it:
            param = gen_param(p)
            if param != '':
                output_string += f'\t\t\t{param}'
                if output_string[-1] != '{':
                    output_string += ';\n'
                else:
                    output_string += '\n'
    return output_string


def gen_paramscast_assign_string(method):
    def gen_param(param):
        if 'ref' in param and param['ref'] is True:
            type = ASS_TYPESCAST_MAP.get(param['type'], 'int')
            name = generate_name(param['name'])
            if 'VectorData' in type:
                size = SIZ_TYPESCAST_MAP.get(param['type'], 'int')
                output = f'Array.Resize(ref {name}, {size}(__{name}));\n'
                output += f'\t\t\t{type}(__{name}, {name})'
                return output
            elif type != '':
                return f'{name} = {type}(__{name})'
            else:
                return ''
        else:
            return ''
    def gen_return(param):
        type = ASS_TYPESCAST_MAP.get(param['type'], 'int')
        if 'VectorData' in type:
            size = SIZ_TYPESCAST_MAP.get(param['type'], 'int')
            return_type = convert_type(param['type'], False)
            output = f'var output = new {return_type[:-1]}{size}(__output)];\n'
            output += f'\t\t\t{type}(__output, output)'
            return output
        elif type != '':
            return f'var output = {type}(__output)'
        else:
            return ''

    output_string = ''
    ret_type = method['retType']
    is_obj_ret = is_obj_return(ret_type["type"])
    if is_obj_ret:
        ret = gen_return(ret_type)
        if ret != '':
            output_string += f'\t\t\t{ret};\n'
    if method["paramTypes"]:
        it = iter(method["paramTypes"])
        param = gen_param(next(it))
        if param != '':
            output_string += f'\t\t\t{param};\n'
        for p in it:
            param = gen_param(p)
            if param != '':
                output_string += f'\t\t\t{param};\n'
    return output_string


def gen_paramscast_assign_string2(method):
    def gen_param(param):
        if 'ref' in param and param['ref'] is True:
            type = ASS_TYPESCAST_MAP.get(param['type'], 'int')
            name = generate_name(param['name'])
            if 'VectorData' in type:
                return ''
            elif type != '':
                return ''
            else:
                return '}'
        else:
            return ''

    output_string = ''
    if method["paramTypes"]:
        it = iter(method["paramTypes"])
        param = gen_param(next(it))
        if param != '':
            output_string += f'\t\t\t{param}\n'
        for p in it:
            param = gen_param(p)
            if param != '':
                output_string += f'\t\t\t{param}\n'
    return output_string


def gen_paramscast_cleanup_string(method):
    def gen_param(param):
        type = DEL_TYPESCAST_MAP.get(param['type'], 'int')
        if type == '':
            return ''
        else:
            return f'{type}(__{generate_name(param["name"])})'

    def gen_return(param):
        type = FRE_TYPESCAST_MAP.get(param['type'], 'int')
        if type == '':
            return ''
        else:
            return f'{type}(__output)'

    output_string = ''
    ret_type = method['retType']
    is_obj_ret = is_obj_return(ret_type['type'])
    if is_obj_ret:
        ret = gen_return(ret_type)
        if ret != '':
            output_string += f'\t\t\t{ret};\n'
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        param = gen_param(next(it))
        if param != '':
            output_string += f'\t\t\t{param};\n'
        for p in it:
            param = gen_param(p)
            if param != '':
                output_string += f'\t\t\t{param};\n'
    return output_string


def gen_paramswrapper_string(method):
    def gen_param(param):
        type = DAT_WRAPPER_MAP.get(param['type'], 'int')
        name = generate_name(param['name'])
        if 'GetString' in type:
            return f'var __{name}__ = {type}(@__{name})'
        elif type != '':
            size = SIZ_WRAPPER_MAP.get(param['type'], 'int')
            output = f'var __{name}__ = new {size}(@__{name})];\n\t\t\t\t'
            output += f'{type}(@__{name}, __{name}__)'
            return output
        else:
            return ''

    output_string = ''
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        param = gen_param(next(it))
        if param != '':
            output_string += f'\t\t\t\t{param};\n'
        for p in it:
            param = gen_param(p)
            if param != '':
                output_string += f'\t\t\t\t{param};\n'
    return output_string


def gen_paramswrapper_assign_string(method):
    def gen_param(param):
        if 'ref' in param and param['ref'] is True:
            type = ASS_WRAPPER_MAP.get(param['type'], 'int')
            name = generate_name(param['name'])
            if type != '':
                return f'{type}(@__{name}, __{name}__)'
            else:
                return ''
        else:
            return ''

    def gen_return(param):
        type = CTR_WRAPPER_MAP.get(param['type'], 'int')
        if 'Construct' in type:
            return f'{type}(@__output, __result__)'
        if type != '':
            return f'{type}(@__output, __result__, __result__.Length)'
        else:
            return ''

    output_string = ''
    ret_type = method['retType']
    is_obj_ret = is_obj_return(ret_type["type"])
    if is_obj_ret:
        ret = gen_return(ret_type)
        if ret != '':
            output_string += f'\t\t\t\t{ret};\n'
    if method["paramTypes"]:
        it = iter(method["paramTypes"])
        param = gen_param(next(it))
        if param != '':
            output_string += f'\t\t\t\t{param};\n'
        for p in it:
            param = gen_param(p)
            if param != '':
                output_string += f'\t\t\t\t{param};\n'
    return output_string


def main(manifest_path, output_dir, override):
    if not os.path.isfile(manifest_path):
        print(f'Manifest file not exists {manifest_path}')
        return 1
    if not os.path.isdir(output_dir):
        print(f'Output folder not exists {output_dir}')
        return 1

    plugin_name = os.path.splitext(os.path.basename(manifest_path))[0]
    header_dir = os.path.join(output_dir, 'pps')
    if not os.path.exists(header_dir):
        os.makedirs(header_dir, exist_ok=True)
    header_file = os.path.join(header_dir, f'{plugin_name}.cs')
    if os.path.isfile(header_file) and not override:
        print(f'Already exists {header_file}')
        return 1

    with open(manifest_path, 'r', encoding='utf-8') as fd:
        pplugin = json.load(fd)

    parse_errors = validate_manifest(pplugin)
    if parse_errors:
        print('Parse fail:')
        for error in parse_errors:
            print(f'  {error}')
        return 1

    content = ''

    link = 'https://github.com/untrustedmodders/plugify-module-dotnet/blob/main/generator/generator.py'

    content += 'using System;\n'
    content += 'using System.Numerics;\n'
    content += 'using System.Runtime.CompilerServices;\n'
    content += 'using System.Runtime.InteropServices;\n'
    content += 'using Plugify;\n'
    content += '\n'
    content += f'//generated with {link} from {plugin_name} \n'
    content += '\n'
    content += f'namespace {plugin_name}\n{{'
    content += '\n'

    delegates = set()

    # Declare delegates
    for method in pplugin['exportedMethods']:
        ret_type = method['retType']
        if "prototype" in ret_type:
            content += decl_wrapper_delegate(ret_type, delegates)
        for param_type in method['paramTypes']:
            if "prototype" in param_type:
                content += decl_wrapper_delegate(param_type, delegates)

    content += f'\n\tinternal static unsafe class {plugin_name}\n\t{{'
    content += '\n'

    content += '\t\tprivate static Dictionary<Delegate, Delegate> s_DelegateHolder = new();\n'
    content += '\t\tprivate static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) where TKey : notnull\n'
    content += '\t\t{\n'
    content += '\t\t\tif (dict.TryGetValue(key, out TValue? val))\n'
    content += '\t\t\t{\n'
    content += '\t\t\t\treturn val;\n'
    content += '\t\t\t}\n'
    content += '\t\t\tdict.Add(key, value);\n'
    content += '\t\t\treturn value;\n'
    content += '\t\t}\n\n'

    for method in pplugin['exportedMethods']:
        content += f'\t\tinternal static delegate* <{gen_types_string(method)}> {method["name"]} = &___{method["name"]};\n'
        content += f'\t\tinternal static delegate* unmanaged[Cdecl]<{gen_ctypes_string(method)}> __{method["name"]};\n'

        ret_type = method['retType']
        return_type = convert_type(ret_type['type'], 'ref' in ret_type and ret_type['ref'] is True)
        if 'delegate' in return_type and 'prototype' in ret_type:
            return_type = generate_name(ret_type['prototype']['name'])
        content += (f'\t\tprivate static {return_type} '
                    f'___{method["name"]}({gen_params_string(method, ParamGen.TypesNames)})\n')
        content += '\t\t{\n'

        for param_type in method['paramTypes']:
            if "prototype" in param_type:
                content += gen_wrapper_body(param_type, method["name"])

        params = gen_paramscast_string(method)
        if params != '':
            content += f'{params}\n'

        is_obj_ret = is_obj_return(ret_type['type'])
        if not is_obj_ret and ret_type['type'] != 'void':
            content += f'\t\t\tvar __result = __{method["name"]}({gen_params_string(method, ParamGen.CastNames)});\n'
        else:
            content += f'\t\t\t__{method["name"]}({gen_params_string(method, ParamGen.CastNames)});\n'

        params = gen_paramscast_assign_string(method)
        if params != '':
            content += f'\n{params}'

        params = gen_paramscast_cleanup_string(method)
        if params != '':
            content += f'\n{params}\n'

        if "prototype" in ret_type:
            content += gen_wrapper_body(ret_type, method["name"])

        if is_obj_ret:
            content += '\t\t\treturn output;\n'
        elif ret_type['type'] == 'function':
            if is_need_marshal(ret_type['prototype']):
                content += f'\t\t\treturn {method["name"]}__result;\n'
            else:
                content += f'\t\t\treturn Marshal.GetDelegateForFunctionPointer<{ret_type["prototype"]["name"]}>(__result);\n'
        elif ret_type['type'] == 'char8' or ret_type['type'] == 'char16':
            content += '\t\t\treturn (char)__result;\n'
        elif ret_type['type'] != 'void':
            content += '\t\t\treturn __result;\n'

        params = gen_paramscast_assign_string2(method)
        if params != '':
            content += f'\n{params}'

        content += '\t\t}\n'
    content += '\t}\n'
    content += '}\n'

    with open(header_file, 'w', encoding='utf-8') as fd:
        fd.write(content)

    return 0


def gen_wrapper_body(param, name):
    content = ''
    prototype = param['prototype']
    if is_need_marshal(prototype):
        content += f'\t\t\tvar {name}_{param["name"]} = ({prototype["name"]}Wrapper) s_DelegateHolder.GetOrAdd({param["name"]}, ({gen_params_string(prototype, ParamGen.WrapperNames)}) => {{\n'

        params = gen_paramswrapper_string(prototype)
        if params != '':
            content += f'{params}\n'

        prot_ret_type = prototype['retType']

        if prot_ret_type['type'] != 'void':
            content += f'\t\t\t\tvar __result__ = {param["name"]}({gen_params_string(prototype, ParamGen.WrapperCastNames)});\n'
        else:
            content += f'\t\t\t\t{param["name"]}({gen_params_string(prototype, ParamGen.WrapperCastNames)});\n'

        params = gen_paramswrapper_assign_string(prototype)
        if params != '':
            content += f'\n{params}'

        if is_obj_return(prot_ret_type['type']):
            content += '\t\t\t\treturn @__output;\n'
        elif prot_ret_type['type'] == 'char8' or prot_ret_type['type'] == 'char16':
            content += '\t\t\t\treturn (char)__result__;\n'
        elif prot_ret_type['type'] != 'void':
            content += '\t\t\t\treturn __result__;\n'

        content += '\t\t\t});\n'
    return content


def decl_wrapper_delegate(param, delegates):
    content = ''
    prototype = param['prototype']
    delegate = gen_delegate(prototype)
    if delegate not in delegates:
        content += delegate
        delegates.add(delegate)
    if is_need_marshal(prototype):
        wrapper_delegate = gen_wrapper_delegate(prototype)
        if wrapper_delegate not in delegates:
            content += wrapper_delegate
            delegates.add(wrapper_delegate)
    return content


def get_args():
    parser = argparse.ArgumentParser()
    parser.add_argument('manifest')
    parser.add_argument('output')
    parser.add_argument('--override', action='store_true')
    return parser.parse_args()


if __name__ == '__main__':
    args = get_args()
    sys.exit(main(args.manifest, args.output, args.override))