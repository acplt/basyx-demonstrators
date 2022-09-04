#include <ccs_type.h>

C3_CC* ccs_type_createInstance(char* name, CCS_TYPE type) {
    if (type >= CCS_TYPE_COUNT)
        return NULL;

    C3_CC *cc = C3_CC_new();
    C3_Info info;
    info.id = name;
    info.name = name;
    info.description = name;
    info.profile = C3_PROFILE_BASYSDEMO;
    info.type = (char*) CCS_TYPE_NAMES[type];
    C3_CC_setInfo(cc, &info);

    CCS_TYPE_FUNCTIONS[type](cc);
    return cc;
}