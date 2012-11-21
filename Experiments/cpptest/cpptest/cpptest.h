// cpptest.h

#pragma once

namespace cpptest {

	public ref class CppComponent : public Kinectitude::Core::Base::Component
	{
		Kinectitude::Core::Data::TypeMatcher ^type;
	public:
		[Kinectitude::Core::Attributes::Plugin("Type matcher", "")]
		property Kinectitude::Core::Data::TypeMatcher ^Type{
			Kinectitude::Core::Data::TypeMatcher ^ get();
			void set(Kinectitude::Core::Data::TypeMatcher ^matcher);
		}
		virtual void OnUpdate(float frameDelta) override;
		virtual void Destroy() override;
	};
}
