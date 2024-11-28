<template>
  <div class="body">


    <div class="container xl:w-[50rem]">
      <input type="checkbox" id="flip">
      <div class="cover">
        <div class="front">
          <img src="/frontImg.jpg" alt="">
          <div class="text">
            <span class="text-1">Streamlining Employee Reimbursement </span>
            <span class="text-2">Transaction Reimbursement</span>
          </div>
        </div>
        <div class="back">
          <img class="backImg" src="/backImg.jpg" alt="">
          <div class="text">
            <span class="text-1">Complete miles of journey <br> with one step</span>
            <span class="text-2">Let's get started</span>
          </div>
        </div>
      </div>
      <div class="forms">
        <div class="form-content">
          <div class="login-form">
            <div class="title">Login</div>
            <form action="#">
              <div class="input-boxes">
                <div class="input-box">
                  <i class="fas fa-envelope"></i>
                  <input type="text" v-model="email" placeholder="Enter your email" required>
                </div>
                <div class="input-box">
                  <i class="fas fa-lock"></i>
                  <input type="password" v-model="password" placeholder="Enter your password" required>
                </div>
                <div class="text"><a href="#">Forgot password?</a></div>
                <div class="button input-box">
                  <input @click="loginMethod" type="submit" value="Submit">
                </div>
                <div class="text sign-up-text">Don't have an account? <RouterLink to="userform" for="flip">Sigup now
                  </RouterLink>
                </div>
              </div>
            </form>
          </div>
          <div class="signup-form">
            <div class="title">Signup</div>
            <form action="#">
              <div class="input-boxes">
                <div class="input-box">
                  <i class="fas fa-user"></i>
                  <input type="text" v-model="username" placeholder="Enter your name" required>
                </div>
                <div class="input-box">
                  <i class="fas fa-envelope"></i>
                  <input type="text" v-model="email" placeholder="Enter your email" required>
                </div>
                <div class="input-box">
                  <i class="fas fa-lock"></i>
                  <input type="password" v-model="password" placeholder="Enter your password" required>
                </div>
                <div class="input-box flex ">
                  <i class="fas fa-lock"></i>
                  <span class="ml-9">Department</span>
                  <select v-model="department" class="px-4 ml-10" name="" id="">
                    <option value=0>HR</option>
                    <option value=1>Finance</option>
                    <option value=2>IT</option>
                    <option value=3>Sales</option>
                    <option value=4>Marketing</option>
                    <option value=7>Admin</option>
                  </select>
                  <!-- <input type="password" placeholder="Enter your password" required> -->
                </div>
                <div class="button input-box">
                  <input type="submit" v-on:click="registerMethod" value="Submit">
                </div>
                <div class="text sign-up-text">Already have an account? <label for="flip">Login now</label></div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script>
import router from '@/scripts/Route';
import { login, register } from '@/scripts/User';
import { useToast } from 'vue-toastification';


export default {
  name: "LoginRegister",
  data() {
    return {
      username: '',
      email: '',
      password: '',
      department: '',
      toast: useToast()

    }

  },
  methods:
  {
    registerMethod() {
      const toast = useToast();
      event.preventDefault();
      register(this.username, this.email, this.password, this.department)
        .then((res) => {
          alert("registration successfull");
          sessionStorage.setItem("token", res.data.token)
          // router.push('/')
          toast.success("registration successfull")
          window.location.reload();
        }, err => {
          console.log(err)
          alert(err);
        })
    },
    async loginMethod() {
      const toastId = this.toast("Loading...",);
      try {

        event.preventDefault();
        // const toast = useToast();


        const res = await login(this.email, this.password)
        sessionStorage.clear();
        sessionStorage.setItem("token", res.data.token)
        sessionStorage.setItem("name", res.data.userName);
        localStorage.setItem("department", res.data.department)
        this.toast.update(toastId, {
          content: "Login Successull!", options: {
            type: "success"
          }
        });


        if (res.data.department == 7) {
          router.push('/admin')

        }
        else {

          router.push('/')
        }

      } catch (err) {
        console.log(err);
        this.toast.update(toastId, {
          content: "Login Failed!", options: {
            type: "error"
          }
        });

      }


    },
  }
}
</script>

<style>
/* Google Font Link */
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@200;300;400;500;600;700&display=swap');

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: "Poppins", sans-serif;
}

.body {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #7d2ae8;
  padding: 30px;
}

.container {
  position: relative;
  max-width: 850px;
  width: 100%;
  background: #fff;
  padding: 40px 30px;
  box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
  perspective: 2700px;
}

.container .cover {
  position: absolute;
  top: 0;
  left: 50%;
  height: 100%;
  width: 50%;
  z-index: 98;
  transition: all 1s ease;
  transform-origin: left;
  transform-style: preserve-3d;
  backface-visibility: hidden;
}

.container #flip:checked~.cover {
  transform: rotateY(-180deg);
}

.container #flip:checked~.forms .login-form {
  pointer-events: none;
}

.container .cover .front,
.container .cover .back {
  position: absolute;
  top: 0;
  left: 0;
  height: 100%;
  width: 100%;
}

.cover .back {
  transform: rotateY(180deg);
}

.container .cover img {
  position: absolute;
  height: 100%;
  width: 100%;
  object-fit: cover;
  z-index: 10;
}

.container .cover .text {
  position: absolute;
  z-index: 10;
  height: 100%;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.container .cover .text::before {
  content: '';
  position: absolute;
  height: 100%;
  width: 100%;
  opacity: 0.5;
  background: #7d2ae8;
}

.cover .text .text-1,
.cover .text .text-2 {
  z-index: 20;
  font-size: 26px;
  font-weight: 600;
  color: #fff;
  text-align: center;
}

.cover .text .text-2 {
  font-size: 15px;
  font-weight: 500;
}

.container .forms {
  height: 100%;
  width: 100%;
  background: #fff;
}

.container .form-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.form-content .login-form,
.form-content .signup-form {
  width: calc(100% / 2 - 25px);
}

.forms .form-content .title {
  position: relative;
  font-size: 24px;
  font-weight: 500;
  color: #333;
}

.forms .form-content .title:before {
  content: '';
  position: absolute;
  left: 0;
  bottom: 0;
  height: 3px;
  width: 25px;
  background: #7d2ae8;
}

.forms .signup-form .title:before {
  width: 20px;
}

.forms .form-content .input-boxes {
  margin-top: 30px;
}

.forms .form-content .input-box {
  display: flex;
  align-items: center;
  height: 50px;
  width: 100%;
  margin: 10px 0;
  position: relative;
}

.form-content .input-box input {
  height: 100%;
  width: 100%;
  outline: none;
  border: none;
  padding: 0 30px;
  font-size: 16px;
  font-weight: 500;
  border-bottom: 2px solid rgba(0, 0, 0, 0.2);
  transition: all 0.3s ease;
}

.form-content .input-box input:focus,
.form-content .input-box input:valid {
  border-color: #7d2ae8;
}

.form-content .input-box i {
  position: absolute;
  color: #7d2ae8;
  font-size: 17px;
}

.forms .form-content .text {
  font-size: 14px;
  font-weight: 500;
  color: #333;
}

.forms .form-content .text a {
  text-decoration: none;
}

.forms .form-content .text a:hover {
  text-decoration: underline;
}

.forms .form-content .button {
  color: #fff;
  margin-top: 40px;
}

.forms .form-content .button input {
  color: #fff;
  background: #7d2ae8;
  border-radius: 6px;
  padding: 0;
  cursor: pointer;
  transition: all 0.4s ease;
}

.forms .form-content .button input:hover {
  background: #5b13b9;
}

.forms .form-content label {
  color: #5b13b9;
  cursor: pointer;
}

.forms .form-content label:hover {
  text-decoration: underline;
}

.forms .form-content .login-text,
.forms .form-content .sign-up-text {
  text-align: center;
  margin-top: 25px;
}

.container #flip {
  display: none;
}

@media (max-width: 730px) {
  .container .cover {
    display: none;
  }

  .form-content .login-form,
  .form-content .signup-form {
    width: 100%;
  }

  .form-content .signup-form {
    display: none;
  }

  .container #flip:checked~.forms .signup-form {
    display: block;
  }

  .container #flip:checked~.forms .login-form {
    display: none;
  }
}
</style>
