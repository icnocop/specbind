
// SampleWin32App.h : main header file for the PROJECT_NAME application
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CSampleWin32App:
// See SampleWin32App.cpp for the implementation of this class
//

class CSampleWin32App : public CWinApp
{
public:
	CSampleWin32App();

// Overrides
public:
	virtual BOOL InitInstance();

// Implementation

	DECLARE_MESSAGE_MAP()
};

extern CSampleWin32App theApp;
