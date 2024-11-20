import axiosIntance from "./Interceptor";

export function register(
  userName,
  email,
  password,
  gender,
  department,
  dateofBirth,
  address
) {
  return axiosIntance.post("/api/User/register", {
    userName,
    email,
    password,
    gender: +gender,
    department: +department,
    dateofBirth,
    address,
  });
}

export function login(email, password) {
  return axiosIntance.post("/api/User/Login", {
    email: email,
    password: password,
  });
}

export function getUserProfile() {
  return axiosIntance.get("/api/User/profile");
}

export function getAllUser(pageNumber = 1, pageSize = 10) {
  return axiosIntance.get("/api/User/users", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function getUserProfileById(id) {
  return axiosIntance.get(`/api/User/${id}`);
}

export function searchUser(searchItem, pageNumber = 1, pageSize = 10) {
  return axiosIntance.get("/api/User/search", {
    params: {
      searchItem,
      pageNumber,
      pageSize,
    },
  });
}
