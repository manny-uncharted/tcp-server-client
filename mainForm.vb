Imports System.Net
Imports System.Net.Sockets
Imports System.Text


Public Class mainForm

    Dim _server As TcpListener
    Dim _listOfClients As New List(Of TcpClient)

    Private Sub startServer_Click(sender As Object, e As EventArgs) Handles startServer.Click
        Try
            Dim ip As String = "127.0.0.1"
            Dim port As Integer = 5432

            _server = New TcpListener(IPAddress.Parse(ip), port)
            _server.Start()

            Threading.ThreadPool.QueueUserWorkItem(AddressOf NewClient)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub NewClient(state As Object)
        Dim client As TcpClient = _server.AcceptTcpClient()
        Try
            _listOfClients.Add(client)
            Threading.ThreadPool.QueueUserWorkItem(AddressOf NewClient)

            While True
                Dim ns As NetworkStream = client.GetStream()

                Dim toReceive(1000000) As Byte

                ns.Read(toReceive, 0, toReceive.Length)
                Dim txt As String = Encoding.ASCII.GetString(toReceive)
                For Each c As TcpClient In _listOfClients
                    If c IsNot client Then
                        Dim nns As NetworkStream = c.GetStream()
                        nns.Write(Encoding.ASCII.GetBytes(txt), 0, txt.Length)
                    End If
                Next
            End While

        Catch ex As Exception
            If _listOfClients.Contains(client) Then
                _listOfClients.Remove(client)
            End If
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
