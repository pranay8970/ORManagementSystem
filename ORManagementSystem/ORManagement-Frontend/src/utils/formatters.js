export const formatDate = (value) => {
  if (!value) return '-'

  const datePart = String(value).substring(0, 10)
  const [year, month, day] = datePart.split('-').map(Number)

  return new Date(year, month - 1, day).toLocaleDateString()
}
export const formatDateTime = (value) => {
  if (!value) return '-'

  return new Date(value).toLocaleString()
}

export const formatTime = (value) => {
  if (!value) return '-'

  if (typeof value === 'string') {
    return value.substring(0, 5)
  }

  return value
}

export const formatPercent = (value) => {
  if (value === null || value === undefined) return '0%'

  return `${Number(value).toFixed(2)}%`
}

export const getErrorMessage = (error, fallback = 'Something went wrong.') => {
  return (
    error?.response?.data?.message ||
    error?.response?.data?.title ||
    fallback
  )
}