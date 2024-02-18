
#ifndef CUSTOMFUNC_INCLUDE
#define CUSTOMFUNC_INCLUDE
#include "Assets/Renderring/Macros.hlsl"

void CheckAndroid_float(float
     input, out float OutPut)
{
     OutPut = input;
     #ifdef ISMOBILE
     OutPut = 1.0-input;
#endif
}

#else


#endif