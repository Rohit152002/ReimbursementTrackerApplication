import axios from "axios";

const axiosIntance = axios.create({
  baseURL: "http://localhost:5286",
  // headers: {
  //   "Content-Type": "multipart/form-data",
  // },
});

function requestInterceptor(config) {
  const token = sessionStorage.getItem("token");
  if (token) {
    config.headers["Authorization"] = `Bearer ${token}`;
  }
  return config;
}

axiosIntance.interceptors.request.use(requestInterceptor);

export default axiosIntance;
