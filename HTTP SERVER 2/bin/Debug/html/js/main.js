if(!document.cookie.includes('id='))
{
    window.location = "login"
} 


const postData = async (url = '', data) => {
    // Формируем запрос
    const response = await fetch(url, {
      method: 'POST',
     
      headers: {
        'Content-Type': 'text'
      },
      body: data
    });
    return response.text()
}
postData('username', document.cookie.split('=')[1]).then((data) => {
    document.getElementById('UserAccName').textContent = data
    document.getElementById('UserAccNameHead').textContent = data
});





let xmr = new XMLHttpRequest()
xmr.open("GET", "posts.txt")

xmr.onload = () => {
    let lst = xmr.response.split("\n")
    let jsons = []
    lst.forEach(element => {
        if(element.includes('{')){
        jsons.push(JSON.parse(element))}
        
    });
    console.log(jsons)


    jsons.forEach(element => {
        let div = document.createElement('div');
        div.className = "post";
        let namePost = document.createElement('span')
        namePost.className = 'name';
        namePost.textContent = element.postname
        div.append(namePost)
        let AthorName = document.createElement('div')
        AthorName.className = 'username'
        AthorName.textContent = element.athour
        div.append(AthorName)
        let content = document.createElement('div')
        content.className = 'content'
        content.textContent = element.content
        div.append(content)
        let div_main = document.getElementById("main");
        div_main.prepend(div)
    });

}

xmr.send();

