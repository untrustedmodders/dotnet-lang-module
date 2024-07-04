
#ifndef PLUGIN_API_H
#define PLUGIN_API_H

#ifdef CPP_TEST_STATIC_DEFINE
#  define PLUGIN_API
#  define CPP_TEST_NO_EXPORT
#else
#  ifndef PLUGIN_API
#    ifdef cpp_test_EXPORTS
        /* We are building this library */
#      define PLUGIN_API __declspec(dllexport)
#    else
        /* We are using this library */
#      define PLUGIN_API __declspec(dllimport)
#    endif
#  endif

#  ifndef CPP_TEST_NO_EXPORT
#    define CPP_TEST_NO_EXPORT 
#  endif
#endif

#ifndef CPP_TEST_DEPRECATED
#  define CPP_TEST_DEPRECATED __declspec(deprecated)
#endif

#ifndef CPP_TEST_DEPRECATED_EXPORT
#  define CPP_TEST_DEPRECATED_EXPORT PLUGIN_API CPP_TEST_DEPRECATED
#endif

#ifndef CPP_TEST_DEPRECATED_NO_EXPORT
#  define CPP_TEST_DEPRECATED_NO_EXPORT CPP_TEST_NO_EXPORT CPP_TEST_DEPRECATED
#endif

#if 0 /* DEFINE_NO_DEPRECATED */
#  ifndef CPP_TEST_NO_DEPRECATED
#    define CPP_TEST_NO_DEPRECATED
#  endif
#endif

#endif /* PLUGIN_API_H */
