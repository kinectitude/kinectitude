// cpptest.h

#pragma once

using namespace System;

namespace cpptest {

	public ref class CppComponent : public Kinectitude::Core::Base::Component
	{
		Kinectitude::Core::Data::ITypeMatcher ^type;
	public:
		[Kinectitude::Core::Attributes::Plugin("Type matcher", "")]
		property Kinectitude::Core::Data::ITypeMatcher ^Type{
			Kinectitude::Core::Data::ITypeMatcher ^ get();
			void set(Kinectitude::Core::Data::ITypeMatcher ^matcher);
		}
		virtual void OnUpdate(float frameDelta) override;
		virtual void Destroy() override;
	};
}
