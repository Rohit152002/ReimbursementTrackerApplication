import axiosIntance from "./Interceptor";

export function getRequests(pageNumber = 1, pageSize = 10) {
  return axiosIntance.get("api/ReimbursementRequest/all", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function submitRequest(form) {
  console.log([...form.entries()]);

  return axiosIntance.post("api/ReimbursementRequest", form);
}

export function getDashboard() {
  return axiosIntance.get("api/ReimbursementRequest/dashboard");
}

export function getRequestByUserId(id, pageNumber = 1, pageSize = 10) {
  return axiosIntance.get(`api/ReimbursementRequest/user/${id}`, {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function getRequestbyManagerId(
  managerId,
  pageNumber = 1,
  pageSize = 10
) {
  return axiosIntance.get(`/api/ReimbursementRequest/manager/${managerId}`, {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function getRequestById(id) {
  return axiosIntance.get(`/api/ReimbursementRequest/${id}`);
}
