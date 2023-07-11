$(function () {
  $(".btn").click(function () {
    $(".form-signin").toggleClass("form-signin-left");
    $(".form-signup").toggleClass("form-signup-left");
    $(".frame").toggleClass("frame-long");
    $(".signup-inactive").toggleClass("signup-active");
    $(".signin-active").toggleClass("signin-inactive");
    $(".forgot").toggleClass("forgot-left");
    $(this).removeClass("idle").addClass("active");
  });
});

$(function () {

  $(".btn-signup").click(function () {
    const postData = async (url = '', data) => {
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)

      });
      return response.text();
    }
    const postData2 = async (url = '', data) => {
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: data

      });
      return response.text();

    }
   
    postData2("Newusername?", document.getElementById("fullname").value).then((data) => {
      let name = document.getElementById("fullname").value;
      let pass = document.getElementById("password").value;
      let cheak = document.getElementById("confirmpassword").value;
      if (pass.length < 6) {
        alert("пароль слишком маленький")
        return;
      }
      if (pass == "" || name == "") {
        alert("все поля должны быть заполнены")
        return;
      }
      if (pass != cheak) {
        alert("пароли не совпадают");
        return;
      }

      if (data == "0") {
        a = confirm("имя занято")
        return
      }
      

      const xhr = new XMLHttpRequest();
      xhr.open("GET", "get_id.txt")
      xhr.onload = () => {
        document.cookie = "id=" + String(Number(xhr.response) + 1);
        postData("new_user", { id: Number(xhr.response) + 1, username: document.getElementById("fullname").value, password: document.getElementById("password").value })


      }
      xhr.send();

      $(".nav").toggleClass("nav-up");
      $(".form-signup-left").toggleClass("form-signup-down");
      $(".success").toggleClass("success-left");
      $(".frame").toggleClass("frame-short");

      function loc(){
        window.location.href = "main"
      }
      setTimeout(loc, 3000)

    });

  });
});


$(function () {
  $(".btn-signin").click(function () {
    
    const postData2 = async (url = '', data) => {
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)

      });
      return response.json();

    }
    postData2("user_log", {user: document.getElementById("username").value, password: document.getElementById("password_login").value}).then((data) =>{
      let name = document.getElementById("username").value;
    let pass = document.getElementById("password_login").value;
    
    if(name == "" || pass == ""){
      alert("все поля должны быть заполнены")
      return;
    }
    let json = data
    if(!data.def){
      alert("неверный пароль или имя")
      return
    }
    document.cookie = "id=" + data.id;




    $(".btn-animate").toggleClass("btn-animate-grow");
    $(".welcome").toggleClass("welcome-left");
    $(".cover-photo").toggleClass("cover-photo-down");
    $(".frame").toggleClass("frame-short");
    $(".profile-photo").toggleClass("profile-photo-down");
    $(".btn-goback").toggleClass("btn-goback-up");
    $(".forgot").toggleClass("forgot-fade");
    function loc(){
      window.location.href = "main"
    }
    setTimeout(loc, 3000)
    });
  });
});