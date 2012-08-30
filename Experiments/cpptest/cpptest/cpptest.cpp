// This is the main DLL file.

#include "stdafx.h"
#include "cpptest.h"

using namespace cpptest;
using namespace Kinectitude::Core::Data;

void CppComponent::OnUpdate(float frameDelta){
	return;
}

void CppComponent::Destroy(){}


ITypeMatcher ^ CppComponent::Type::get() {
	return type;
}

void CppComponent::Type::set(ITypeMatcher ^ matcher){
	if(matcher != type){
		type = matcher;
		Change("Type");
	}
}
