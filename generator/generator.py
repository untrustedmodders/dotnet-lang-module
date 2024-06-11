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
    'ptr64': 'IntPtr',
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
    'ptr64*': 'IntPtr[]',
    'float*': 'float[]',
    'double*': 'double[]',
    'string*': 'string[]',
    'vec2': 'Vector2',
    'vec3': 'Vector3',
    'vec4': 'Vector4',
    'mat4x4': 'Matrix4x4'
}

CTYPES_MAP = {
    'void': 'void',
    'bool': 'bool',
    'char8': 'sbyte',
    'char16': 'char',
    'int8': 'sbyte',
    'int16': 'short',
    'int32': 'int',
    'int64': 'long',
    'uint8': 'byte',
    'uint16': 'ushort',
    'uint32': 'uint',
    'uint64': 'ulong',
    'ptr64': 'IntPtr',
    'float': 'float',
    'double': 'double',
    'function': 'IntPtr',
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
    'vec2': 'Vector2',
    'vec3': 'Vector3',
    'vec4': 'Vector4',
    'mat4x4': 'Matrix4x4'
}

VAL_TYPESCAST_MAP = {
    'void': '',
    'bool': '',
    'char8': 'Convert.ToSByte',
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
    'string': 'MethodNatives.CreateString',
    'bool*': 'MethodNatives.CreateVectorBool',
    'char8*': 'MethodNatives.CreateVectorChar8',
    'char16*': 'MethodNatives.CreateVectorChar16',
    'int8*': 'MethodNatives.CreateVectorInt8',
    'int16*': 'MethodNatives.CreateVectorInt16',
    'int32*': 'MethodNatives.CreateVectorInt32',
    'int64*': 'MethodNatives.CreateVectorInt64',
    'uint8*': 'MethodNatives.CreateVectorUInt8',
    'uint16*': 'MethodNatives.CreateVectorUInt16',
    'uint32*': 'MethodNatives.CreateVectorUInt32',
    'uint64*': 'MethodNatives.CreateVectorUInt64',
    'ptr64*': 'MethodNatives.CreateVectorUIntPtr',
    'float*': 'MethodNatives.CreateVectorFloat',
    'double*': 'MethodNatives.CreateVectorDouble',
    'string*': 'MethodNatives.CreateVectorString',
    'vec2': '',
    'vec3': '',
    'vec4': '',
    'mat4x4': ''
}

RET_TYPESCAST_MAP = {
    'void': '',
    'bool': '',
    'char8': 'Convert.ToSByte',
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
    'string': 'MethodNatives.AllocateString',
    'bool*': 'MethodNatives.AllocateVectorBool',
    'char8*': 'MethodNatives.AllocateVectorChar8',
    'char16*': 'MethodNatives.AllocateVectorChar16',
    'int8*': 'MethodNatives.AllocateVectorInt8',
    'int16*': 'MethodNatives.AllocateVectorInt16',
    'int32*': 'MethodNatives.AllocateVectorInt32',
    'int64*': 'MethodNatives.AllocateVectorInt64',
    'uint8*': 'MethodNatives.AllocateVectorUInt8',
    'uint16*': 'MethodNatives.AllocateVectorUInt16',
    'uint32*': 'MethodNatives.AllocateVectorUInt32',
    'uint64*': 'MethodNatives.AllocateVectorUInt64',
    'ptr64*': 'MethodNatives.AllocateVectorUIntPtr',
    'float*': 'MethodNatives.AllocateVectorFloat',
    'double*': 'MethodNatives.AllocateVectorDouble',
    'string*': 'MethodNatives.AllocateVectorString',
    'vec2': 'Vector2',
    'vec3': 'Vector3',
    'vec4': 'Vector4',
    'mat4x4': 'Matrix4x4'
}

ASS_TYPESCAST_MAP = {
    'void': '',
    'bool': '',
    'char8': 'Convert.ToChar',
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
    'string': 'MethodNatives.GetStringData',
    'bool*': 'MethodNatives.GetVectorDataBool',
    'char8*': 'MethodNatives.GetVectorDataChar8',
    'char16*': 'MethodNatives.GetVectorDataChar16',
    'int8*': 'MethodNatives.GetVectorDataInt8',
    'int16*': 'MethodNatives.GetVectorDataInt16',
    'int32*': 'MethodNatives.GetVectorDataInt32',
    'int64*': 'MethodNatives.GetVectorDataInt64',
    'uint8*': 'MethodNatives.GetVectorDataUInt8',
    'uint16*': 'MethodNatives.GetVectorDataUInt16',
    'uint32*': 'MethodNatives.GetVectorDataUInt32',
    'uint64*': 'MethodNatives.GetVectorDataUInt64',
    'ptr64*': 'MethodNatives.GetVectorDataUIntPtr',
    'float*': 'MethodNatives.GetVectorDataFloat',
    'double*': 'MethodNatives.GetVectorDataDouble',
    'string*': 'MethodNatives.GetVectorDataString',
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
    'bool*': 'MethodNatives.GetVectorSizeBool',
    'char8*': 'MethodNatives.GetVectorSizeChar8',
    'char16*': 'MethodNatives.GetVectorSizeChar16',
    'int8*': 'MethodNatives.GetVectorSizeInt8',
    'int16*': 'MethodNatives.GetVectorSizeInt16',
    'int32*': 'MethodNatives.GetVectorSizeInt32',
    'int64*': 'MethodNatives.GetVectorSizeInt64',
    'uint8*': 'MethodNatives.GetVectorSizeUInt8',
    'uint16*': 'MethodNatives.GetVectorSizeUInt16',
    'uint32*': 'MethodNatives.GetVectorSizeUInt32',
    'uint64*': 'MethodNatives.GetVectorSizeUInt64',
    'ptr64*': 'MethodNatives.GetVectorSizeUIntPtr',
    'float*': 'MethodNatives.GetVectorSizeFloat',
    'double*': 'MethodNatives.GetVectorSizeDouble',
    'string*': 'MethodNatives.GetVectorSizeString',
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
    'string': 'MethodNatives.DeleteString',
    'bool*': 'MethodNatives.DeleteVectorBool',
    'char8*': 'MethodNatives.DeleteVectorChar8',
    'char16*': 'MethodNatives.DeleteVectorChar16',
    'int8*': 'MethodNatives.DeleteVectorInt8',
    'int16*': 'MethodNatives.DeleteVectorInt16',
    'int32*': 'MethodNatives.DeleteVectorInt32',
    'int64*': 'MethodNatives.DeleteVectorInt64',
    'uint8*': 'MethodNatives.DeleteVectorUInt8',
    'uint16*': 'MethodNatives.DeleteVectorUInt16',
    'uint32*': 'MethodNatives.DeleteVectorUInt32',
    'uint64*': 'MethodNatives.DeleteVectorUInt64',
    'ptr64*': 'MethodNatives.DeleteVectorUIntPtr',
    'float*': 'MethodNatives.DeleteVectorFloat',
    'double*': 'MethodNatives.DeleteVectorDouble',
    'string*': 'MethodNatives.DeleteVectorString',
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
    'string': 'MethodNatives.FreeString',
    'bool*': 'MethodNatives.FreeVectorBool',
    'char8*': 'MethodNatives.FreeVectorChar8',
    'char16*': 'MethodNatives.FreeVectorChar16',
    'int8*': 'MethodNatives.FreeVectorInt8',
    'int16*': 'MethodNatives.FreeVectorInt16',
    'int32*': 'MethodNatives.FreeVectorInt32',
    'int64*': 'MethodNatives.FreeVectorInt64',
    'uint8*': 'MethodNatives.FreeVectorUInt8',
    'uint16*': 'MethodNatives.FreeVectorUInt16',
    'uint32*': 'MethodNatives.FreeVectorUInt32',
    'uint64*': 'MethodNatives.FreeVectorUInt64',
    'ptr64*': 'MethodNatives.FreeVectorUIntPtr',
    'float*': 'MethodNatives.FreeVectorFloat',
    'double*': 'MethodNatives.FreeVectorDouble',
    'string*': 'MethodNatives.FreeVectorString',
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


def convert_ctype(type_name, is_ref=False):
    type = CTYPES_MAP.get(type_name, 'int')
    if is_ref:
        if type == '*':
            return 'IntPtr'
        else:
            return 'ref ' + type
    else:
        if type == '*':
            return 'IntPtr'
        else:
            return type

 
def is_obj_type(type_name):
    return '*' in type_name or type_name == 'string'

 
def is_obj_return(type_name):
    return '*' in type_name or type_name == 'string' or 'vec' in type_name or 'mat' in type_name 


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


def gen_params_string(method, param_gen: ParamGen):
    def gen_param(param):
        if param_gen == ParamGen.Types:
            type = convert_type(param['type'], 'ref' in param and param['ref'] is True)
            if 'delegate' in type and 'prototype' in param:
                type = generate_name(param['prototype']['name'])
            return type
        elif param_gen == ParamGen.Names:
            return generate_name(param['name'])
        elif param_gen == ParamGen.CastNames:
            if is_obj_type(param['type']):
                return 'C_' + generate_name(param['name'])
            elif param['type'] == 'char8':
                if 'ref' in param and param['ref'] is True:
                    return 'ref C_' + generate_name(param['name'])
                else:
                    return 'C_' + generate_name(param['name'])
            else:
                if 'ref' in param and param['ref'] is True:
                    return 'ref ' + generate_name(param['name'])
                else:
                    return generate_name(param['name'])
        type = convert_type(param['type'], 'ref' in param and param['ref'] is True)
        if 'delegate' in type and 'prototype' in param:
            type = generate_name(param['prototype']['name'])
        return f'{type} {generate_name(param['name'])}'
        
    def gen_return(param):
        type = param['type']
        if 'vec' in type or 'mat' in type:
            return f'ref C_output'
        else:
            return 'C_output' 
        
    output_string = ''
    ret_type = method['retType']
    is_obj_ret = is_obj_return(ret_type['type']) and param_gen == ParamGen.CastNames
    if is_obj_ret:
        output_string += f'{gen_return(ret_type)}'
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        if not is_obj_ret:
            output_string += gen_param(next(it))
        for p in it:
            output_string += f', {gen_param(p)}'
    return output_string


def gen_types_string(method):
    output_string = ''
    ret_type = method['retType']
    obj_return = is_obj_return(ret_type['type']) 
    if obj_return:
        output_string += f'{convert_ctype(ret_type['type'], True)}'
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        if not obj_return:
            param = next(it)
            output_string += convert_ctype(param['type'], 'ref' in param and param['ref'] is True)
        for p in it:
            output_string += f', {convert_ctype(p['type'], 'ref' in p and p['ref'] is True)}'
    if output_string != '':
        output_string += ', '
    if obj_return:
        output_string += 'void'
    else:
        output_string += f'{convert_ctype(ret_type['type'])}'
    return output_string
    

def gen_paramscast_string(method):
    def gen_param(param):
        type = VAL_TYPESCAST_MAP.get(param['type'], 'int')
        name = generate_name(param['name'])
        if 'CreateVector' in type:
            return f'var C_{name} = {type}({name}, {name}.Length);'
        elif  type != '':
            return f'var C_{name} = {type}({name});'
        else:
            return '';
      
    def gen_return(param):
        type = RET_TYPESCAST_MAP.get(param['type'], 'int')
        return f'var C_output = {type}()'

    output_string = ''
    ret_type = method['retType']
    is_obj_ret = is_obj_return(ret_type['type'])
    if is_obj_ret:
        output_string += f'\t\t\t{gen_return(ret_type)}'
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        param = gen_param(next(it))
        if param != '':
            output_string += f'\t\t\t{param}\n'
        for p in it:
            param = gen_param(p)
            if param != '':
                output_string += f'\t\t\t{param}\n'
    return output_string
             
             
def gen_paramscast_assign_string(method):
    def gen_param(param):
        if 'ref' in param and param['ref'] is True:
            type = ASS_TYPESCAST_MAP.get(param['type'], 'int')
            name = generate_name(param['name'])
            if 'VectorData' in type:
                size = SIZ_TYPESCAST_MAP.get(param['type'], 'int')
                output = f'Array.Resize(ref {name}, {size}(C_{name}));\n'
                output += f'\t\t\t{type}(C_{name}, {name});'
                return output
            elif type != '':
                return f'{name} = {type}(C_{name})'
            else:
                return ''
        else:
            return ''
    def gen_return(param):
        type = ASS_TYPESCAST_MAP.get(param['type'], 'int')
        if 'VectorData' in type:
            size = SIZ_TYPESCAST_MAP.get(param['type'], 'int')
            return_type = convert_type(param['type'], False)
            output = f'var output = new {return_type[:-1]}{size}()];\n'
            output += f'\t\t\t{type}(C_output, output);'
            return output
        elif type != '':
            return f'var output = {type}(C_output)'
        else:
                return ''

    output_string = ''
    ret_type = method['retType']
    is_obj_ret = is_obj_return(ret_type["type"])
    if is_obj_ret:
        output_string += f'\t\t\t{gen_return(ret_type)}'
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
            return f'{type}(C_{generate_name(param['name'])})'

    def gen_return(param):
        type = FRE_TYPESCAST_MAP.get(param['type'], 'int')
        if type == '':
            return ''
        else:
            return f'{type}(C_output)'

    output_string = ''
    ret_type = method['retType']
    is_obj_ret = is_obj_return(ret_type['type'])
    if is_obj_ret:
        output_string += f'\t\t\t{gen_return(ret_type)}'
    if method['paramTypes']:
        it = iter(method['paramTypes'])
        param = gen_param(next(it))
        if param != '':
            output_string += f'\t\t\t{param}\n'
        for p in it:
            param = gen_param(p)
            if param != '':
                output_string += f'\t\t\t{param}\n'
    return output_string             


def gen_delegate(prototype):
    ret_type = prototype['retType']
    return_type = convert_type(ret_type['type'], 'ref' in ret_type and ret_type['ref'] is True)
    return (f'\tdelegate {return_type} '
            f'{prototype['name']}({gen_params_string(prototype, ParamGen.TypesNames)});\n')


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

    link = 'https://github.com/untrustedmodders/dotnet-lang-module/blob/main/generator/generator.py'

    content += 'using System;\n'
    content += 'using System.Runtime.CompilerServices;\n'
    content += 'using System.Runtime.InteropServices;\n'
    content += 'using Plugify;\n'
    content += '\n'
    content += f'//generated with {link} from {plugin_name} \n'
    content += '\n'
    content += f'namespace {plugin_name}\n{{'
    content += '\n'
    
    for method in pplugin['exportedMethods']:
        ret_type = method['retType']
        if 'prototype' in ret_type:
            content += gen_delegate(ret_type['prototype'])
        for attribute in method['paramTypes']:
            if 'prototype' in attribute:
                content += gen_delegate(attribute['prototype'])

    content += f'\n\tinternal static unsafe class {plugin_name}\n\t{{'
    content += '\n'
    for method in pplugin['exportedMethods']:
        content += f'\t\tprivate static IntPtr {method['name']}Ptr = IntPtr.Zero;\n'
    
        ret_type = method['retType']
        return_type = convert_type(ret_type['type'], 'ref' in ret_type and ret_type['ref'] is True)
        content += (f'\t\tinternal static {return_type} '
                    f'{method['name']}({gen_params_string(method, ParamGen.TypesNames)})\n')
        content += '\t\t{\n'
        
        params = gen_paramscast_string(method)
        if params != '':
            content += f'{params}\n'
        
        content += f'\t\t\tif ({method['name']}Ptr == IntPtr.Zero) {method['name']}Ptr = NativeMethods.GetMethodPtr("{method['name']}");\n'
        content += f'\t\t\tvar {method['name']}Func = (delegate* unmanaged[Cdecl]<{gen_types_string(method)}>){method['name']}Ptr;\n'
        
        is_obj_ret = is_obj_return(ret_type['type'])
        if not is_obj_ret and ret_type['type'] != 'void':
            content += f'\t\t\tvar result = {method['name']}Func({gen_params_string(method, ParamGen.CastNames)});\n'
        else:
            content += f'\t\t\t{method['name']}Func({gen_params_string(method, ParamGen.CastNames)});\n'
            
        params = gen_paramscast_assign_string(method)
        if params != '':
            content += f'\n{params}'
        
        params = gen_paramscast_cleanup_string(method)
        if params != '':
            content += f'\n{params}\n'
        
        if 'vec' in ret_type['type'] or 'mat' in ret_type['type'] :
            content += '\t\t\treturn C_output\n'
        elif is_obj_ret:
            content += '\t\t\treturn output\n'
        elif ret_type['type'] != 'void':
            content += '\t\t\treturn result\n'
        
        content += '\t\t}\n'                  
    content += '\t}\n'
    content += '}\n'

    with open(header_file, 'w', encoding='utf-8') as fd:
        fd.write(content)

    return 0


def get_args():
    parser = argparse.ArgumentParser()
    parser.add_argument('manifest')
    parser.add_argument('output')
    parser.add_argument('--override')
    return parser.parse_args()


if __name__ == '__main__':
    args = get_args()
    sys.exit(main(args.manifest, args.output, args.override))