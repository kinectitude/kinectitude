// This is the main DLL file.

#include "stdafx.h"
#include "cpptest.h"

using namespace cpptest;
using namespace Kinectitude::Core::Data;

void CppComponent::OnUpdate(float frameDelta){
	return;
}

void CppComponent::Destroy(){}

TypeMatcher ^ CppComponent::Type::get() {
	return type;
}

void CppComponent::Type::set(TypeMatcher ^ matcher){
	if(matcher != type){
		type = matcher;
		Change("Type");
	}
}
