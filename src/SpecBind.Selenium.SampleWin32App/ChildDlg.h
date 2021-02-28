#pragma once
#include "afxwin.h"
class ChildDlg : public CDialog
{
public:
    ChildDlg();

    // Dialog Data
#ifdef AFX_DESIGN_TIME
    enum { IDD = IDD_CHILD_DIALOG };
#endif

protected:
    virtual void DoDataExchange( CDataExchange* pDX );    // DDX/DDV support

                                                          // Implementation
protected:
    DECLARE_MESSAGE_MAP()
};

