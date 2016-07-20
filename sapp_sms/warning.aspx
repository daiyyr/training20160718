<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="warning.aspx.cs" Inherits="sapp_sms.warning" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sapp SMS - Warning!</title>
    <link href="styles/master.css" rel="stylesheet" type="text/css" />
    <link href="styles/sub.css" rel="stylesheet" type="text/css" />
</head>
<body id="masterbody">
    <form id="form1" runat="server">
    <div id="container">
        <div id="header">
            <div id="left_logo">
                <asp:Image ID="ImageLogo" runat="server" CssClass="ImageLogo"
                    ImageUrl="~/images/simpleapp-logo-t.png" ImageAlign="Middle"/>
            </div>
            <div id="middle_msg">
            </div>
            <div id="right_login">
            </div>
        </div>
        <div id="main">
            <div id="menu">
                
            </div>
            <div id="content">
                <div id="login">
                <table class="login">
                    <tr>
                       <td align="center">
                        <table>
                            <tr>
                                <td colspan="2"><b>You do not have permission to access this page!</b></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div class="button">
                                        <div class="button-title">Home</div>
                                        <div>
                                            <asp:ImageButton ID="ImageButtonHome"
                                                runat="server" ImageUrl="Images/home.gif" 
                                                onclick="ImageButtonHome_Click"/>
                                        </div>
                                        <div>
                                            <asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Button ID="ButtonLogin" runat="server" Text="Login" 
                                        onclick="ButtonLogin_Click"/>
                                </td>
                            </tr>
                        </table>
                       </td>
                    </tr>
                </table>
                </div>
        </div>
        <div id="rooter">
            
        </div>
    </div>
    </form>
</body>
</html>
