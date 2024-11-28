import axiosIntance from "./Interceptor";

export function getAllCategories(pageNumber, pageSize) {
  return axiosIntance.get("/api/Category/all", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}
