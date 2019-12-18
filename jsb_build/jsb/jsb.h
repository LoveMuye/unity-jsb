
#if defined(JSB_F_WINDOWS)
    #if defined(JSB_COMPILING)
        #define JSB_EXTERNAL_DECL extern __declspec(dllexport)
        #define JSB_EXTERNAL __declspec(dllexport)
    #else
        #define JSB_EXTERNAL_DECL extern __declspec(dllimport)
        #define JSB_EXTERNAL
    #endif
#else
    #define JSB_EXTERNAL_DECL  __attribute__ ((visibility("default"))) extern
    #define JSB_EXTERNAL       __attribute__ ((visibility("default")))
#endif

struct JSBClass {
    JSClassID class_id;
    // JSValue prototype;
    // JSValue constructor;
    JSClassDef class_def;
};

struct JSBVM {
    JSRuntime *rt;
    JSContext *ctx;
};

JSB_EXTERNAL_DECL struct JSBVM *JSB_NewVM();
JSB_EXTERNAL_DECL void JSB_FreeVM(struct JSBVM *vm);