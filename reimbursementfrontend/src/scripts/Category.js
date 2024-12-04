import axiosIntance from "./Interceptor";

export function getAllCategories(pageNumber, pageSize) {
  return axiosIntance.get("/api/Category/all", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function addCategory(name, description) {
  return axiosIntance.post("/api/Category", {
    name,
    description,
  });
}

export function editCategory(id, name, description) {
  return axiosIntance.put(`/api/Category/${id}`, {
    name,
    description,
  });
}
