<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="sapp_sms.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sapp SMS v3</title>
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
                                <td colspan="2">
                                    <img src="images/skyline.gif" width="250px" /> 
                                </td>
                            </tr>
                            <tr>
                                <td>Login:</td>
                                <td>
                                    <asp:TextBox ID="TextBoxLogin" runat="server" MaxLength="20" Width="150px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Password:</td>
                                <td>
                                    <asp:TextBox ID="TextBoxPassword" runat="server" MaxLength="8" 
                                        TextMode="Password" Width="150px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Bodycorps:</td>
                                <td>
                                    <asp:DropDownList ID="DropDownList1" runat="server" Width="150px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Button ID="ButtonLogin" runat="server" Text="Login" onclick="ButtonLogin_Click" 
                                         />
                                </td>
                            </tr>
                        </table>
                       </td>
                    </tr>
                </table>
                </div>
            </div>
        </div>
        <div id="rooter">
            
        </div>
    </div>
    </form>
</body>
</html>
