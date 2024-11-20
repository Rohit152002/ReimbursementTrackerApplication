import axiosIntance from "./Interceptor";

export function getEmployee(pageNumber = 1, pageSize = 10) {
  return axiosIntance.get("/api/Employee/Employees", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function getUserWithNoManager(pageNumber = 1, pageSize = 10) {
  return axiosIntance.get("/api/Employee/withoutmanager", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function assignManagerRequest(employeeId, managerId) {
  return axiosIntance.post("/api/Employee", {
    employeeId,
    managerId,
  });
}
