import {AxiosError, AxiosResponse} from "axios"
import type {AxiosInstance} from "axios"
import {injectable, inject} from "inversify"

import "@/extensions/date.extensions"
import {ApiService} from "@/services/apiService"
import {IQuizService} from "@/injection/interfaces"
import {TYPES} from "@/injection/types"

export enum QuizQuestionType {
  Scale1To10 = 0,
  MultipleChoice = 1,
  TextInput = 2,
  MultipleSelection = 3
}

export interface Quiz {
  id: string
  titre: string
  description?: string
  imageUrl?: string
  dueDate?: Date
  questions: QuizQuestion[]
}

export interface QuizQuestion {
  id: string
  questionText: string
  order: number
  questionType: QuizQuestionType
  placeholder?: string
  scaleLabels?: string[]
  responses: QuizResponse[]
}

export interface QuizResponse {
  id: string
  responseText: string
  order: number
}

export interface AssignedQuiz {
  id: string
  quizId: string
  version: number
  followUpLabel?: string
  titre: string
  description?: string
  imageUrl?: string
  assignedAt: Date
  availableAt?: Date
  dueDate?: Date
  completedAt?: Date
  isCompleted: boolean
}

export interface SubmitQuizResponseRequest {
  quizAssignmentId: string
  quizQuestionId: string
  selectedScore?: number
  selectedResponseId?: string
  selectedResponseIds?: string[]
  selectedTextResponse?: string
}

export interface SubmitQuizResponse {
  quizQuestionId: string
  questionType: QuizQuestionType
  questionText: string
  isValid: boolean
  message?: string
}

export interface CreateQuizRequest {
  titre: string
  description?: string
  imageUrl?: string
  questions: CreateQuizQuestionRequest[]
}

export interface CreateQuizQuestionRequest {
  questionText: string
  order: number
  questionType: QuizQuestionType
  placeholder?: string
  scaleMinLabel?: string
  scaleMidLabel?: string
  scaleMaxLabel?: string
  responses: CreateQuizResponseRequest[]
}

export interface CreateQuizResponseRequest {
  responseText: string
  order: number
}

export interface UpdateQuizRequest {
  id: string
  titre: string
  description?: string
  imageUrl?: string
}

export interface CreateQuestionRequest {
  quizId: string
  questionText: string
  order: number
  questionType: QuizQuestionType
  placeholder?: string
  responses: CreateResponseRequest[]
}

export interface CreateQuizQuestionRequest {
  questionText: string
  order: number
  questionType: QuizQuestionType
  placeholder?: string
  scaleMinLabel?: string
  scaleMidLabel?: string
  scaleMaxLabel?: string
  responses: CreateResponseRequest[]
}

export interface CreateResponseRequest {
  responseText: string
  order: number
}

export interface UpdateQuestionRequest {
  questionId: string
  questionText: string
  order: number
  questionType: QuizQuestionType
  placeholder?: string
  scaleMinLabel?: string
  scaleMidLabel?: string
  scaleMaxLabel?: string
  responses: UpdateResponseRequest[]
}

export interface UpdateResponseRequest {
  id?: string
  responseText: string
  order: number
}

@injectable()
export class QuizService extends ApiService implements IQuizService {
  constructor(@inject(TYPES.AxiosInstance) httpClient: AxiosInstance) {
    super(httpClient)
  }

  public async getAll(): Promise<Quiz[]> {
    try {
      const response = await this
        ._httpClient
        .get<Quiz[], AxiosResponse<Quiz[]>>(`${import.meta.env.VITE_API_BASE_URL}/quiz`)
      return response.data
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async getById(id: string): Promise<Quiz> {
    try {
      const response = await this
        ._httpClient
        .get<Quiz, AxiosResponse<Quiz>>(`${import.meta.env.VITE_API_BASE_URL}/quiz/${id}`)
      return response.data
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async getAssignedQuizzes(): Promise<AssignedQuiz[]> {
    try {
      const response = await this
        ._httpClient
        .get<AssignedQuiz[], AxiosResponse<AssignedQuiz[]>>(`${import.meta.env.VITE_API_BASE_URL}/quiz/assigned`)
      return response.data
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async create(quiz: CreateQuizRequest): Promise<void> {
    try {
      await this
        ._httpClient
        .post(`${import.meta.env.VITE_API_BASE_URL}/quiz`, quiz)
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async update(quiz: UpdateQuizRequest): Promise<void> {
    try {
      await this
        ._httpClient
        .put(`${import.meta.env.VITE_API_BASE_URL}/quiz/${quiz.id}`, quiz)
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async assignQuiz(quizId: string, userIds: string[], followUpLabel?: string, availableAt?: Date, dueDate?: Date): Promise<void> {
    try {
      await this
        ._httpClient
        .post(`${import.meta.env.VITE_API_BASE_URL}/quiz/${quizId}/assign`, {
          quizId,
          userIds,
          followUpLabel,
          availableAt,
          dueDate
        })
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async submitResponse(response: SubmitQuizResponseRequest): Promise<SubmitQuizResponse> {
    try {
      const payload = {
        ...response,
        selectedResponseIds: response.selectedResponseIds ?? []
      }

      const result = await this
        ._httpClient
        .post<SubmitQuizResponse, AxiosResponse<SubmitQuizResponse>>(`${import.meta.env.VITE_API_BASE_URL}/quiz/submit-response`, payload)
      return result.data
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async delete(id: string): Promise<void> {
    try {
      await this
        ._httpClient
        .delete(`${import.meta.env.VITE_API_BASE_URL}/quiz/${id}`)
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async createQuestion(question: CreateQuestionRequest): Promise<void> {
    try {
      await this
        ._httpClient
        .post(`${import.meta.env.VITE_API_BASE_URL}/quiz/${question.quizId}/questions`, question)
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async updateQuestion(question: UpdateQuestionRequest): Promise<void> {
    try {
      await this
        ._httpClient
        .put(`${import.meta.env.VITE_API_BASE_URL}/quiz/questions/${question.questionId}`, question)
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async deleteQuestion(questionId: string): Promise<void> {
    try {
      await this
        ._httpClient
        .delete(`${import.meta.env.VITE_API_BASE_URL}/quiz/questions/${questionId}`)
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async getUserResponses(quizAssignmentId: string): Promise<any> {
    try {
      const response = await this
        ._httpClient
        .get(`${import.meta.env.VITE_API_BASE_URL}/quiz/assignments/${quizAssignmentId}/responses`)
      return response.data
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async completeQuiz(quizAssignmentId: string): Promise<void> {
    try {
      await this
        ._httpClient
        .post(`${import.meta.env.VITE_API_BASE_URL}/quiz/assignments/${quizAssignmentId}/complete`, { quizAssignmentId })
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async getAssignments(quizId: string): Promise<{ id: string; userId: string; version: number; followUpLabel?: string; availableAt?: string; dueDate?: string; completedAt?: string }[]> {
    try {
      const response = await this
        ._httpClient
        .get<{ id: string; userId: string; version: number; followUpLabel?: string; availableAt?: string; dueDate?: string; completedAt?: string }[], AxiosResponse<{ id: string; userId: string; version: number; followUpLabel?: string; availableAt?: string; dueDate?: string; completedAt?: string }[]>>(`${import.meta.env.VITE_API_BASE_URL}/quiz/${quizId}/assignments`)
      return response.data
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async unassignQuiz(quizId: string, userIds: string[]): Promise<void> {
    try {
      await this
        ._httpClient
        .post(`${import.meta.env.VITE_API_BASE_URL}/quiz/${quizId}/unassign`, {
          quizId,
          userIds
        })
    } catch (error) {
      return Promise.reject(error)
    }
  }
}
