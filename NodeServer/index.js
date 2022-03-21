const WebSocket = require('ws');
const wss = new WebSocket.Server({port: 8080}, ()=>{
  console.log('Server Started.......')
})

wss.on('connection', (socket)=>{
  console.log("Player conectado");
  socket.on('message', async function message(data){
    await socket.send('Take it back --> ' + data);
    console.log('Data received...' + data);
  })
})



wss.on('listening', ()=>{
  console.log('server is listening on port 8080.......')
})
